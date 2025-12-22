USE [MedicalReports_BK]
GO
/****** Object:  StoredProcedure [dbo].[PopulateGuestSchedule_Latest]    Script Date: 12/22/2025 5:12:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[PopulateGuestSchedule_Latest]
    @GuestId INT=16041
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Variables for Guest's Arrival and Departure
    DECLARE @DateOfArrival DATETIME;
    DECLARE @DateOfDepartment DATETIME;
    DECLARE @CheckinProcessStart DATETIME;
    DECLARE @CheckinProcessEnd DATETIME;
    DECLARE @SettleDownStart DATETIME;
    DECLARE @SettleDownEnd DATETIME;
    DECLARE @PackingStart DATETIME;
    DECLARE @PackingEnd DATETIME;
    DECLARE @CheckoutProcessStart DATETIME;
    DECLARE @CheckoutProcessEnd DATETIME;

    -- Fetch DateOfArrival and DateOfDepartment for the specified GuestId
    -- Priority: RoomAllocation.CheckInDate first, then MembersDetails.DateOfArrival
    -- Priority: RoomAllocation.CheckOutDate first, then MembersDetails.DateOfDepartment
    SELECT 
        @DateOfArrival = (Case when ra.CheckInDate is null then md.DateOfArrival else ra.CheckInDate end),
        @DateOfDepartment = (Case when ra.CheckOutDate is null then md.DateOfDepartment else ra.CheckOutDate end)
    FROM MembersDetails md
    Left Join RoomAllocation ra on md.Id=ra.GuestID and ra.IsActive=1
    WHERE md.Id = @GuestId;

    -- Check if the guest exists
    IF @DateOfArrival IS NULL OR @DateOfDepartment IS NULL
    BEGIN
        RAISERROR ('Invalid GuestId or missing date information.', 16, 1);
        RETURN;
    END;

    -- Calculate check-in task windows
    -- CheckinProcess: Starts at arrival, lasts 15 minutes
    SET @CheckinProcessStart = @DateOfArrival;
    SET @CheckinProcessEnd = DATEADD(MINUTE, 15, @DateOfArrival);
    
    -- SettleDown: Starts after CheckinProcess ends, lasts 45 minutes
    SET @SettleDownStart = @CheckinProcessEnd;
    SET @SettleDownEnd = DATEADD(MINUTE, 45, @SettleDownStart);

    -- Calculate checkout task windows
    -- Packing: Starts 60 minutes before checkout, lasts 45 minutes
    SET @PackingStart = DATEADD(MINUTE, -60, @DateOfDepartment);
    SET @PackingEnd = DATEADD(MINUTE, -16, @DateOfDepartment); -- Ends 1 minute before CheckoutProcess starts
    
    -- CheckoutProcess: Starts 15 minutes before checkout, lasts 15 minutes
    SET @CheckoutProcessStart = DATEADD(MINUTE, -15, @DateOfDepartment);
    SET @CheckoutProcessEnd = @DateOfDepartment;

    -- Step 2: Cancel tasks that overlap with check-in/checkout windows or are before check-in/after checkout
    -- Cancel tasks that:
    -- 1. Overlap with CheckinProcess window (@CheckinProcessStart to @CheckinProcessEnd)
    -- 2. Overlap with SettleDown window (@SettleDownStart to @SettleDownEnd)
    -- 3. Overlap with Packing window (@PackingStart to @PackingEnd)
    -- 4. Overlap with CheckoutProcess window (@CheckoutProcessStart to @CheckoutProcessEnd)
    -- 5. Are scheduled BEFORE check-in (tasks ending before or at arrival time)
    -- 6. Are scheduled AFTER checkout (tasks starting after checkout time)
    -- Exclude check-in/checkout tasks (TaskId 6, 7, 8, 9) as they will be created/updated below
    UPDATE GuestSchedule
    SET IsCancelled = 1
    WHERE GuestId = @GuestId
        AND TaskId NOT IN (6, 7, 8, 9)  -- Exclude check-in/checkout tasks
        AND (
            -- Overlap with CheckinProcess window
            (StartDateTime < @CheckinProcessEnd AND EndDateTime > @CheckinProcessStart)
            OR
            -- Overlap with SettleDown window
            (StartDateTime < @SettleDownEnd AND EndDateTime > @SettleDownStart)
            OR
            -- Overlap with Packing window
            (StartDateTime < @PackingEnd AND EndDateTime > @PackingStart)
            OR
            -- Overlap with CheckoutProcess window
            (StartDateTime < @CheckoutProcessEnd AND EndDateTime > @CheckoutProcessStart)
            OR
            -- Tasks scheduled BEFORE check-in (ending before or at arrival time)
            (EndDateTime <= @DateOfArrival)
            OR
            -- Tasks scheduled AFTER checkout (starting after checkout time)
            (StartDateTime > @DateOfDepartment)
        )
        AND (IsCancelled IS NULL OR IsCancelled = 0)
        AND (IsDeleted IS NULL OR IsDeleted = 0);

    -- Step 3: Insert Check-In Process (only if it doesn't already exist)
    IF NOT EXISTS (
        SELECT 1 FROM GuestSchedule 
        WHERE GuestId = @GuestId AND TaskId = 6
        AND (IsCancelled IS NULL OR IsCancelled = 0)
        AND (IsDeleted IS NULL OR IsDeleted = 0)
    )
    BEGIN
        INSERT INTO GuestSchedule (GuestId, StartDateTime, EndDateTime, Duration, TaskId)
        VALUES
        (
            @GuestId,
            @CheckinProcessStart,
            @CheckinProcessEnd,
            '00:15:00',
            6 -- For Check in process
        );
    END
    ELSE
    BEGIN
        -- Update existing CheckinProcess schedule
        UPDATE GuestSchedule
        SET StartDateTime = @CheckinProcessStart,
            EndDateTime = @CheckinProcessEnd,
            Duration = '00:15:00',
            IsCancelled = 0,
            IsDeleted = 0
        WHERE GuestId = @GuestId AND TaskId = 6;
    END;

    -- Step 4: Insert Settle Down (only if it doesn't already exist)
    IF NOT EXISTS (
        SELECT 1 FROM GuestSchedule 
        WHERE GuestId = @GuestId AND TaskId = 8
        AND (IsCancelled IS NULL OR IsCancelled = 0)
        AND (IsDeleted IS NULL OR IsDeleted = 0)
    )
    BEGIN
        INSERT INTO GuestSchedule (GuestId, StartDateTime, EndDateTime, Duration, TaskId)
        VALUES
        (
            @GuestId,
            @SettleDownStart,
            @SettleDownEnd,
            '00:45:00',
            8 -- Settle Down
        );
    END
    ELSE
    BEGIN
        -- Update existing SettleDown schedule
        UPDATE GuestSchedule
        SET StartDateTime = @SettleDownStart,
            EndDateTime = @SettleDownEnd,
            Duration = '00:45:00',
            IsCancelled = 0,
            IsDeleted = 0
        WHERE GuestId = @GuestId AND TaskId = 8;
    END;

    -- Step 5: Insert Schedules from MasterSchedule for each day
    DECLARE @CurrentDate DATETIME = @DateOfArrival;
    WHILE @CurrentDate <= DATEADD(DAY, -1, @DateOfDepartment)
    BEGIN
        INSERT INTO GuestSchedule (GuestId, StartDateTime, EndDateTime, Duration, TaskId)
        SELECT
            @GuestId AS GuestId,
            DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.StartTime AS DATETIME)) AS StartDateTime,
            DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.EndTime AS DATETIME)) AS EndDateTime,
            ms.Duration,
            ms.TaskId AS TaskId
        FROM 
            MasterSchedule ms
        WHERE 
            ms.IsActive = 1
            AND DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.StartTime AS DATETIME)) >= @SettleDownEnd
            AND DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.EndTime AS DATETIME)) <= @PackingStart
            AND ms.TaskId NOT IN (6, 8, 9, 7)
            -- Avoid duplicates: only insert if schedule doesn't already exist for this guest, task, and time
            AND NOT EXISTS (
                SELECT 1 FROM GuestSchedule gs
                WHERE gs.GuestId = @GuestId
                    AND gs.TaskId = ms.TaskId
                    AND gs.StartDateTime = DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.StartTime AS DATETIME))
                    AND gs.EndDateTime = DATEADD(DAY, DATEDIFF(DAY, 0, @CurrentDate), CAST(ms.EndTime AS DATETIME))
                    AND (gs.IsCancelled IS NULL OR gs.IsCancelled = 0)
                    AND (gs.IsDeleted IS NULL OR gs.IsDeleted = 0)
            );

        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END;

    -- Step 6: Insert Packing (only if it doesn't already exist)
    IF NOT EXISTS (
        SELECT 1 FROM GuestSchedule 
        WHERE GuestId = @GuestId AND TaskId = 9
        AND (IsCancelled IS NULL OR IsCancelled = 0)
        AND (IsDeleted IS NULL OR IsDeleted = 0)
    )
    BEGIN
        INSERT INTO GuestSchedule (GuestId, StartDateTime, EndDateTime, Duration, TaskId)
        VALUES
        (
            @GuestId,
            @PackingStart,
            @PackingEnd,
            '00:45:00',
            9 -- Packing
        );
    END
    ELSE
    BEGIN
        -- Update existing Packing schedule
        UPDATE GuestSchedule
        SET StartDateTime = @PackingStart,
            EndDateTime = @PackingEnd,
            Duration = '00:45:00',
            IsCancelled = 0,
            IsDeleted = 0
        WHERE GuestId = @GuestId AND TaskId = 9;
    END;

    -- Step 7: Insert Check-Out Process (only if it doesn't already exist)
    IF NOT EXISTS (
        SELECT 1 FROM GuestSchedule 
        WHERE GuestId = @GuestId AND TaskId = 7
        AND (IsCancelled IS NULL OR IsCancelled = 0)
        AND (IsDeleted IS NULL OR IsDeleted = 0)
    )
    BEGIN
        INSERT INTO GuestSchedule (GuestId, StartDateTime, EndDateTime, Duration, TaskId)
        VALUES
        (
            @GuestId,
            @CheckoutProcessStart,
            @CheckoutProcessEnd,
            '00:15:00',
            7 -- Check Out Process
        );
    END
    ELSE
    BEGIN
        -- Update existing CheckoutProcess schedule
        UPDATE GuestSchedule
        SET StartDateTime = @CheckoutProcessStart,
            EndDateTime = @CheckoutProcessEnd,
            Duration = '00:15:00',
            IsCancelled = 0,
            IsDeleted = 0
        WHERE GuestId = @GuestId AND TaskId = 7;
    END;
END
GO


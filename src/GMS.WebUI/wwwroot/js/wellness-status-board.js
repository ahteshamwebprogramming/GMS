/**
 * Wellness Treatment Status Board - GMS
 * Dynamic operations board with real-time updates
 */

(function () {
    'use strict';

    // ============ CONFIGURATION ============
    const REFRESH_INTERVAL = 30000; // 30 seconds
    const DEMO_MODE = false; // Set to true for demo/testing

    // Status mappings
    const STATUS_MAP = {
        0: { text: 'Pending', class: 'pending' },
        1: { text: 'Pending', class: 'pending' },
        2: { text: 'Pending', class: 'pending' },
        3: { text: 'In Progress', class: 'in-progress' },
        4: { text: 'Completed', class: 'completed' },
        5: { text: 'Cancelled', class: 'cancelled' }
    };

    const MS_PER_MINUTE = 60_000;
    const MS_PER_HOUR = 3_600_000;
    const START_SOON_MINUTES = 10;

    // ============ DOM ELEMENTS ============
    let statusTable = null;
    let statusTableBody = null;
    let lastSyncElement = null;
    let refreshInterval = null;

    // ============ STATE ============
    let currentData = [];

    // ============ INITIALIZATION ============
    function init() {
        statusTable = document.getElementById('statusTable');
        statusTableBody = document.getElementById('statusTableBody');
        lastSyncElement = document.getElementById('lastSyncTime');

        if (!statusTable) return;

        // Load initial data from server-rendered config
        if (window.WellnessBoardConfig && window.WellnessBoardConfig.initialData) {
            currentData = window.WellnessBoardConfig.initialData;
        }

        // Initialize status displays for existing rows
        initializeStatusDisplays();

        // Update last sync time
        updateLastSync();

        // Start auto-refresh
        startAutoRefresh();

        // Update dynamic statuses periodically (every 30 seconds)
        setInterval(updateDynamicStatuses, 30000);

        // Update elapsed timers every second
        setInterval(updateElapsedTimers, 1000);
    }

    // ============ STATUS RENDERING ============
    function renderStatus(container, statusText) {
        if (!container) return;

        const className = statusText.toLowerCase().replace(/\s+/g, '-');
        const currentStatus = container.getAttribute('data-current-status');
        
        // Skip if status hasn't changed
        if (currentStatus === statusText) return;

        container.setAttribute('data-current-status', statusText);
        container.innerHTML = '';

        const chars = statusText.split('');
        chars.forEach((char, index) => {
            const cell = document.createElement('div');
            cell.className = 'char-cell';

            const text = document.createElement('div');
            text.className = `char-text ${className}`;
            text.textContent = char;

            cell.appendChild(text);
            container.appendChild(cell);

            // Staggered animation
            setTimeout(() => text.classList.add('show'), index * 40);
        });
    }

    function initializeStatusDisplays() {
        updateDynamicStatuses();
    }

    function formatTimeDiff(diffMs) {
        const absDiff = Math.abs(diffMs);
        const mins = Math.floor(absDiff / MS_PER_MINUTE);
        if (mins > 59) {
            const hrs = Math.floor(mins / 60);
            const remMins = mins % 60;
            return remMins > 0 ? `${hrs}h ${remMins}m` : `${hrs}h`;
        }
        return `${mins}m`;
    }

    function formatElapsedTime(diffMs) {
        const absDiff = Math.abs(diffMs);
        const hours = Math.floor(absDiff / MS_PER_HOUR).toString().padStart(2, '0');
        const minutes = Math.floor((absDiff % MS_PER_HOUR) / MS_PER_MINUTE).toString().padStart(2, '0');
        const seconds = Math.floor((absDiff % MS_PER_MINUTE) / 1000).toString().padStart(2, '0');
        return `${hours}:${minutes}:${seconds}`;
    }

    function updateElapsedTimers() {
        const now = Date.now();
        
        // Update in-progress elapsed timers
        const inProgressRows = statusTableBody?.querySelectorAll('tr[data-status="in-progress"]');
        inProgressRows?.forEach(row => {
            const startedAtStr = row.getAttribute('data-started-at');
            const scheduledTimeStr = row.getAttribute('data-scheduled-time');
            const startedAt = startedAtStr ? new Date(startedAtStr) : null;
            const scheduledTime = scheduledTimeStr ? new Date(scheduledTimeStr) : null;
            const effectiveStart = startedAt || scheduledTime;
            const remarkCell = row.querySelector('.cell-remark');

            if (effectiveStart && remarkCell) {
                const elapsed = now - effectiveStart.getTime();
                if (elapsed > 0) {
                    remarkCell.textContent = `Elapsed: ${formatElapsedTime(elapsed)}`;
                    remarkCell.title = `Elapsed: ${formatElapsedTime(elapsed)}`;
                }
            }
        });

        // Update starting soon countdown timers
        const startingSoonRows = statusTableBody?.querySelectorAll('tr.starting-soon');
        startingSoonRows?.forEach(row => {
            const scheduledTimeStr = row.getAttribute('data-scheduled-time');
            const scheduledTime = scheduledTimeStr ? new Date(scheduledTimeStr) : null;
            const remarkCell = row.querySelector('.cell-remark');

            if (scheduledTime && remarkCell) {
                const timeUntil = scheduledTime.getTime() - now;
                if (timeUntil > 0) {
                    remarkCell.textContent = `will start in ${formatElapsedTime(timeUntil)}`;
                    remarkCell.title = `will start in ${formatElapsedTime(timeUntil)}`;
                }
            }
        });
    }

    function updateDynamicStatuses() {
        const now = new Date();
        const rows = statusTableBody?.querySelectorAll('tr[data-schedule-id]');
        if (!rows) return;

        rows.forEach(row => {
            const statusAttr = row.getAttribute('data-status');
            const scheduledTimeStr = row.getAttribute('data-scheduled-time');
            const scheduledTime = scheduledTimeStr ? new Date(scheduledTimeStr) : null;
            const container = row.querySelector('[data-status-container]');
            const remarkCell = row.querySelector('.cell-remark');

            if (!container) return;

            // Skip completed, cancelled, in-progress
            if (statusAttr === 'completed' || statusAttr === 'cancelled') {
                renderStatus(container, statusAttr === 'completed' ? 'Completed' : 'Cancelled');
                // Remove is-overdue class if present
                row.classList.remove('is-overdue');
                return;
            }

            if (statusAttr === 'in-progress') {
                renderStatus(container, 'In Progress');
                // Remove is-overdue class - in-progress should never be overdue
                row.classList.remove('is-overdue');
                // Show elapsed time for in-progress using actual start time
                const startedAtStr = row.getAttribute('data-started-at');
                const startedAt = startedAtStr ? new Date(startedAtStr) : null;
                const effectiveStart = startedAt || scheduledTime;
                
                if (effectiveStart && remarkCell) {
                    const elapsed = now - effectiveStart;
                    if (elapsed > 0) {
                        remarkCell.textContent = `Elapsed: ${formatTimeDiff(elapsed)}`;
                        remarkCell.title = `Elapsed: ${formatTimeDiff(elapsed)}`;
                    }
                }
                return;
            }

            // For pending/scheduled/assigned statuses, calculate dynamic status
            if (scheduledTime) {
                const diffMs = now.getTime() - scheduledTime.getTime();

                if (diffMs > 0) {
                    // Overdue - past scheduled time
                    renderStatus(container, 'Overdue');
                    if (remarkCell) {
                        remarkCell.textContent = `delayed by ${formatTimeDiff(diffMs)}`;
                        remarkCell.title = `delayed by ${formatTimeDiff(diffMs)}`;
                    }
                    row.classList.add('is-overdue');
                    row.classList.remove('starting-soon');
                } else {
                    const minsUntilStart = Math.abs(diffMs) / MS_PER_MINUTE;
                    
                    if (minsUntilStart <= START_SOON_MINUTES) {
                        // Starting Soon - within 10 minutes
                        renderStatus(container, 'Starting Soon');
                        if (remarkCell) {
                            remarkCell.textContent = `starts in ${formatTimeDiff(diffMs)}`;
                            remarkCell.title = `starts in ${formatTimeDiff(diffMs)}`;
                        }
                        row.classList.add('starting-soon');
                    } else {
                        // Scheduled - starts in future
                        renderStatus(container, 'Scheduled');
                        if (remarkCell) {
                            remarkCell.textContent = `starts in ${formatTimeDiff(diffMs)}`;
                            remarkCell.title = `starts in ${formatTimeDiff(diffMs)}`;
                        }
                        row.classList.remove('starting-soon');
                    }
                    row.classList.remove('is-overdue');
                }
            } else {
                renderStatus(container, 'Scheduled');
            }
        });
    }

    // ============ DATA REFRESH ============
    function startAutoRefresh() {
        if (DEMO_MODE) return;
        
        refreshInterval = setInterval(refreshData, REFRESH_INTERVAL);
    }

    async function refreshData() {
        if (!window.WellnessBoardConfig || !window.WellnessBoardConfig.refreshUrl) return;

        try {
            const url = `${window.WellnessBoardConfig.refreshUrl}?boardDate=${window.WellnessBoardConfig.boardDate}`;
            const response = await fetch(url);
            
            if (!response.ok) throw new Error('Failed to fetch data');
            
            const newData = await response.json();
            updateTable(newData);
            updateLastSync();
        } catch (error) {
            console.error('Error refreshing wellness board data:', error);
        }
    }

    function updateTable(newData) {
        if (!statusTableBody) return;

        // Filter out completed records (status 4 = Completed)
        const filteredData = newData.filter(s => s.status !== 4);

        // Remove no-data row if it exists and we have data
        const noDataRow = document.getElementById('noDataRow');
        if (noDataRow && filteredData && filteredData.length > 0) {
            noDataRow.remove();
        }

        // Remove rows that are now completed
        const existingRows = statusTableBody.querySelectorAll('tr[data-schedule-id]');
        existingRows.forEach(row => {
            const scheduleId = row.getAttribute('data-schedule-id');
            const scheduleInNew = newData.find(s => s.scheduleId == scheduleId);
            if (scheduleInNew && scheduleInNew.status === 4) {
                row.remove();
            }
        });

        // Update existing rows or add new ones
        filteredData.forEach(schedule => {
            const existingRow = statusTableBody.querySelector(`tr[data-schedule-id="${schedule.scheduleId}"]`);
            
            if (existingRow) {
                updateRow(existingRow, schedule);
            } else {
                const newRow = createRow(schedule);
                statusTableBody.appendChild(newRow);
                // Initialize status display for new row
                const container = newRow.querySelector('[data-status-container]');
                if (container) {
                    const statusInfo = STATUS_MAP[schedule.status] || STATUS_MAP[0];
                    renderStatus(container, statusInfo.text);
                }
            }
        });

        // Update current data (store filtered data)
        currentData = filteredData;

        // Update dynamic statuses after table refresh
        updateDynamicStatuses();
    }

    function updateRow(row, schedule) {
        const statusInfo = STATUS_MAP[schedule.status] || STATUS_MAP[0];
        const currentStatusAttr = row.getAttribute('data-status');
        
        // Check if status changed
        if (currentStatusAttr !== statusInfo.class) {
            row.setAttribute('data-status', statusInfo.class);
            
            // Update status display with animation
            const container = row.querySelector('[data-status-container]');
            if (container) {
                container.setAttribute('data-status', statusInfo.class);
                renderStatus(container, statusInfo.text);
            }

            // Add row update animation
            row.classList.add('row-updated');
            setTimeout(() => row.classList.remove('row-updated'), 2000);
        }

        // Update actual start time if available
        if (schedule.actualStartTime) {
            row.setAttribute('data-started-at', schedule.actualStartTime);
        }

        // Update overdue status - Only mark as overdue if status is NOT in-progress, completed, or cancelled
        const scheduledTime = new Date(schedule.scheduledDateTime);
        const statusClass = statusInfo.class;
        // Only mark as overdue if status is scheduled, assigned, or pending (not in-progress, completed, cancelled)
        const canBeOverdue = statusClass !== 'in-progress' && statusClass !== 'completed' && statusClass !== 'cancelled';
        const isOverdue = canBeOverdue && schedule.status !== 4 && schedule.status !== 5 && new Date() > scheduledTime;
        row.classList.toggle('is-overdue', isOverdue);

        // Update remarks if changed
        const remarkCell = row.querySelector('.cell-remark');
        if (remarkCell) {
            const newRemark = schedule.remarks || '-';
            const issuePrefix = schedule.issueReported ? '<span class="issue-badge">⚠</span>' : '';
            if (remarkCell.textContent.trim() !== newRemark) {
                remarkCell.innerHTML = issuePrefix + newRemark;
                remarkCell.title = schedule.remarks || '';
            }
        }
    }

    function createRow(schedule) {
        const statusInfo = STATUS_MAP[schedule.status] || STATUS_MAP[0];
        const scheduledTime = new Date(schedule.scheduledDateTime);
        const isOverdue = schedule.status !== 4 && schedule.status !== 5 && new Date() > scheduledTime;
        
        const row = document.createElement('tr');
        row.setAttribute('data-schedule-id', schedule.scheduleId);
        row.setAttribute('data-status', statusInfo.class);
        row.setAttribute('data-scheduled-time', schedule.scheduledDateTime);
        if (schedule.actualStartTime) {
            row.setAttribute('data-started-at', schedule.actualStartTime);
        }
        if (isOverdue) row.classList.add('is-overdue');

        const timeStr = scheduledTime.toLocaleTimeString('en-US', { 
            hour: '2-digit', 
            minute: '2-digit',
            hour12: true 
        });

        const guestInfo = schedule.guestName || 'N/A';
        const roomInfo = schedule.roomNo ? ` <small class="text-muted">(Room ${schedule.roomNo})</small>` : '';
        const issuePrefix = schedule.issueReported ? '<span class="issue-badge">⚠</span>' : '';

        row.innerHTML = `
            <td class="cell-truncate" title="${schedule.therapistName || 'Unassigned'}">
                ${schedule.therapistName || 'Unassigned'}
            </td>
            <td class="cell-truncate" title="${schedule.treatmentName || 'N/A'}">
                ${schedule.treatmentName || 'N/A'}
            </td>
            <td>${timeStr}</td>
            <td>${schedule.treatmentRoom || 'TBD'}</td>
            <td class="cell-truncate" title="${guestInfo}${schedule.roomNo ? ` (Room ${schedule.roomNo})` : ''}">
                ${guestInfo}${roomInfo}
            </td>
            <td>
                <div class="status-container" data-status-container data-status="${statusInfo.class}"></div>
            </td>
            <td class="cell-remark" title="${schedule.remarks || ''}">
                ${issuePrefix}${schedule.remarks || '-'}
            </td>
        `;

        return row;
    }


    // ============ LAST SYNC ============
    function updateLastSync() {
        if (!lastSyncElement) return;

        const now = new Date();
        const timeStr = now.toLocaleTimeString('en-US', { 
            hour: '2-digit', 
            minute: '2-digit',
            hour12: true 
        });

        lastSyncElement.textContent = timeStr;
    }

    // ============ PUBLIC API ============
    window.WellnessBoard = {
        refresh: refreshData,
        updateLastSync: updateLastSync,
        getStatus: () => currentData
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

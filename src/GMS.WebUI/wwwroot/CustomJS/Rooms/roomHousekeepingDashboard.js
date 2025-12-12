(function () {
    const MANUAL_WORK_DATE_KEY = 'hkManualWorkDate';

    const state = {
        workDate: null,
        apiBase: null,
        unassignedRooms: [],
        matrixRows: [],
        team: [],
        modalRooms: [],
        modalFilteredRooms: [],
        modalSelected: new Set(),
        currentWorker: null,
        modalInstance: null,
        datePickerInstance: null
    };

    document.addEventListener('DOMContentLoaded', function () {
        init();
    });

    async function init() {
        if (!window.hkDashboardModel) {
            return;
        }

        state.workDate = window.hkDashboardModel.workDate;
        state.apiBase = window.hkDashboardModel.apiBase;
        state.unassignedRooms = window.hkDashboardModel.initialUnassigned || [];
        state.matrixRows = window.hkDashboardModel.initialMatrix || [];
        state.team = window.hkDashboardModel.initialTeam || [];

        await ensureClientDateDefault();

        initFlatpickr();
        initExportButton();
        initRefreshButton();
        initDateForm();
        initTeamFilters();
        initUnassignedFilters();
        initMatrixFilters();
        initAssignModal();
        initWorkerButtons();
    }

    function initFlatpickr(options = {}) {
        if (state.datePickerInstance) {
            state.datePickerInstance.destroy();
            state.datePickerInstance = null;
        }
        if (!window.flatpickr) {
            return;
        }
        const input = document.getElementById('hkDateDisplay');
        if (!input) {
            return;
        }
        state.datePickerInstance = flatpickr(input, {
            defaultDate: parseWorkDate(state.workDate),
            allowInput: false,
            dateFormat: 'd-M-Y',
            onReady: function (selectedDates, dateStr, instance) {
                if (selectedDates && selectedDates[0]) {
                    instance.input.value = instance.formatDate(selectedDates[0], 'd-M-Y');
                }
            },
            onChange: function (selectedDates, dateStr, instance) {
                const hidden = document.getElementById('hkWorkDate');
                if (hidden) {
                    if (selectedDates && selectedDates[0]) {
                        hidden.value = instance.formatDate(selectedDates[0], 'Y-m-d');
                    } else {
                        hidden.value = state.workDate;
                    }
                }
                sessionStorage.setItem(MANUAL_WORK_DATE_KEY, '1');
                const fallback = parseWorkDate(state.workDate).toISOString().split('T')[0];
                const newDate = hidden
                    ? hidden.value
                    : (selectedDates && selectedDates[0]
                        ? instance.formatDate(selectedDates[0], 'Y-m-d')
                        : fallback);
                if (newDate) {
                    refreshDashboardData(newDate);
                }
            }
        });
    }

    async function ensureClientDateDefault() {
        try {
            const params = new URLSearchParams(window.location.search || '');
            const queryValue = params.get('workDate');
            const todayIso = new Date().toISOString().split('T')[0];
            const manualSelection = sessionStorage.getItem(MANUAL_WORK_DATE_KEY) === '1';

            if (manualSelection) {
                return false;
            }

            if (queryValue === todayIso) {
                return false;
            }

            sessionStorage.setItem(MANUAL_WORK_DATE_KEY, '0');
            await refreshDashboardData(todayIso, { skipHistory: true, suppressErrors: true });
            return true;
        } catch (err) {
            console.error('Failed to normalize default work date:', err);
            return false;
        }
    }

    function initExportButton() {
        const btn = document.getElementById('hkExportBtn');
        if (!btn) return;
        btn.addEventListener('click', function () {
            alert('Export report is coming soon.');
        });
    }

    function initRefreshButton() {
        const btn = document.getElementById('hkRefreshBtn');
        if (!btn) return;
        btn.addEventListener('click', function () {
            if (state.workDate) {
                refreshDashboardData(state.workDate, { skipHistory: true });
            }
        });
    }

    function initDateForm() {
        const form = document.getElementById('hkDateForm');
        if (!form) return;
        form.addEventListener('submit', function (evt) {
            evt.preventDefault();
        });
    }

    function initTeamFilters() {
        const search = document.getElementById('teamSearch');
        const dept = document.getElementById('teamDepartment');
        if (search) {
            search.addEventListener('input', filterTeamCards);
        }
        if (dept) {
            dept.addEventListener('change', filterTeamCards);
        }
    }

    function filterTeamCards() {
        const search = normalize(document.getElementById('teamSearch')?.value);
        const dept = normalize(document.getElementById('teamDepartment')?.value);
        const cards = document.querySelectorAll('#hkTeamCards .hk-worker-card');
        let visibleCount = 0;

        cards.forEach(card => {
            const name = normalize(card.dataset.workerName);
            const department = normalize(card.dataset.department);
            const matchesSearch = !search || name.includes(search);
            const matchesDept = !dept || department.includes(dept);
            if (matchesSearch && matchesDept) {
                card.removeAttribute('hidden');
                visibleCount++;
            } else {
                card.setAttribute('hidden', 'hidden');
            }
        });

        toggleEmptyState('hkTeamEmpty', visibleCount === 0);
    }

    function initUnassignedFilters() {
        ['unassignedSearch', 'unassignedRoomType', 'unassignedStatus', 'unassignedTidy'].forEach(id => {
            bindFilterControl(id, filterUnassignedCards);
        });
    }

    function filterUnassignedCards() {
        const search = normalize(document.getElementById('unassignedSearch')?.value);
        const roomType = normalize(document.getElementById('unassignedRoomType')?.value);
        const status = normalize(document.getElementById('unassignedStatus')?.value);
        const tidy = normalize(document.getElementById('unassignedTidy')?.value);

        const cards = document.querySelectorAll('#hkUnassignedCards .hk-room-card');
        let visible = 0;
        cards.forEach(card => {
            const matchesSearch = !search || normalize(card.dataset.roomNumber).includes(search);
            const matchesType = !roomType || normalize(card.dataset.roomType).includes(roomType);
            const matchesStatus = !status || normalize(card.dataset.availability) === status;
            const matchesTidy = !tidy || normalize(card.dataset.tidy) === tidy;
            if (matchesSearch && matchesType && matchesStatus && matchesTidy) {
                card.removeAttribute('hidden');
                visible++;
            } else {
                card.setAttribute('hidden', 'hidden');
            }
        });

        toggleEmptyState('hkUnassignedEmpty', visible === 0);
    }

    function initMatrixFilters() {
        ['matrixRoomSearch', 'matrixRoomType', 'matrixAssignedTo', 'matrixStatus'].forEach(id => {
            bindFilterControl(id, filterMatrixRows);
        });
    }

    function filterMatrixRows() {
        const search = normalize(document.getElementById('matrixRoomSearch')?.value);
        const type = normalize(document.getElementById('matrixRoomType')?.value);
        const assignedTo = normalize(document.getElementById('matrixAssignedTo')?.value);
        const status = normalize(document.getElementById('matrixStatus')?.value);

        const rows = document.querySelectorAll('#hkMatrixTable tbody tr');
        let visible = 0;
        rows.forEach(row => {
            const matchesRoom = !search || normalize(row.dataset.roomNumber).includes(search);
            const matchesType = !type || normalize(row.dataset.roomType).includes(type);
            const matchesAssigned = !assignedTo || normalize(row.dataset.assignedTo).includes(assignedTo);
            const matchesStatus = !status || normalize(row.dataset.status) === status;
            if (matchesRoom && matchesType && matchesAssigned && matchesStatus) {
                row.removeAttribute('hidden');
                visible++;
            } else {
                row.setAttribute('hidden', 'hidden');
            }
        });
        toggleEmptyState('hkMatrixEmpty', visible === 0);
    }

    function initWorkerButtons() {
        const container = document.getElementById('hkTeamCards');
        if (!container) return;
        container.addEventListener('click', function (evt) {
            const btn = evt.target.closest('.hk-open-assignment');
            if (!btn) return;
            const workerId = parseInt(btn.dataset.workerId, 10);
            state.currentWorker = {
                id: workerId,
                name: btn.dataset.workerName,
                code: btn.dataset.workerCode
            };
            openAssignModal();
        });
    }

    function initAssignModal() {
        const modalEl = document.getElementById('assignRoomsModal');
        if (!modalEl) return;
        state.modalInstance = new bootstrap.Modal(modalEl);
        document.getElementById('assignSelectAll')?.addEventListener('change', toggleSelectAllModal);
        document.getElementById('assignSearch')?.addEventListener('input', filterModalRooms);
        document.getElementById('assignSubmitBtn')?.addEventListener('click', submitAssignments);
        modalEl.addEventListener('hidden.bs.modal', function () {
            resetModalState();
            state.currentWorker = null;
        });
    }

    function openAssignModal() {
        if (!state.modalInstance || !state.currentWorker) return;

        // Update worker info
        document.getElementById('assignWorkerName').textContent = state.currentWorker.name || 'N/A';
        document.getElementById('assignWorkerCode').textContent = state.currentWorker.code || 'N/A';

        // Update work date
        const workDateEl = document.getElementById('assignWorkDate');
        if (workDateEl && state.workDate) {
            const date = new Date(state.workDate);
            const formatted = date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
            workDateEl.textContent = formatted.replace(/ /g, '-');
        }

        // Clear previous selections without dropping worker info
        clearModalSelections();
        setModalLoading(true);

        // Format workDate properly (ensure it's in YYYY-MM-DD format)
        let workDateParam = state.workDate;
        if (typeof workDateParam === 'string') {
            // If it's already in the right format, use it; otherwise parse it
            if (!/^\d{4}-\d{2}-\d{2}$/.test(workDateParam)) {
                const date = new Date(workDateParam);
                if (!isNaN(date.getTime())) {
                    workDateParam = date.toISOString().split('T')[0];
                }
            }
        } else if (workDateParam instanceof Date) {
            workDateParam = workDateParam.toISOString().split('T')[0];
        }

        // Fetch fresh unassigned rooms
        fetch(`${state.apiBase}/unassigned?workDate=${encodeURIComponent(workDateParam)}`)
            .then(res => {
                if (!res.ok) {
                    throw new Error(`HTTP ${res.status}: Unable to load unassigned rooms.`);
                }
                return res.json();
            })
            .then(data => {
                if (Array.isArray(data) && data.length > 0) {
                    state.modalRooms = data;
                } else if (state.unassignedRooms && state.unassignedRooms.length > 0) {
                    state.modalRooms = state.unassignedRooms;
                } else {
                    state.modalRooms = [];
                }
                renderModalRooms(state.modalRooms);
                setModalLoading(false);
            })
            .catch(err => {
                console.error('Error fetching unassigned rooms:', err);
                // Fallback to cached data
                if (state.unassignedRooms && state.unassignedRooms.length > 0) {
                    state.modalRooms = state.unassignedRooms;
                    renderModalRooms(state.modalRooms);
                    showModalError('Unable to refresh room list. Showing cached data.');
                } else {
                    state.modalRooms = [];
                    renderModalRooms(state.modalRooms);
                    showModalError('No unassigned rooms available. Please try again later.');
                }
                setModalLoading(false);
            });

        state.modalInstance.show();
    }

    function renderModalRooms(rooms) {
        const list = document.getElementById('assignRoomList');
        const empty = document.getElementById('assignRoomEmpty');

        // Clear selections
        state.modalSelected.clear();
        const selectAll = document.getElementById('assignSelectAll');
        if (selectAll) {
            selectAll.checked = false;
            selectAll.indeterminate = false;
        }

        if (!rooms || rooms.length === 0) {
            list.innerHTML = '';
            empty?.removeAttribute('hidden');
            return;
        }

        empty?.setAttribute('hidden', 'hidden');

        // Handle both camelCase and PascalCase property names
        list.innerHTML = rooms.map(room => {
            const roomId = room.roomId || room.RoomId || 0;
            const roomNumber = room.roomNumber || room.RoomNumber || '';
            const roomType = room.roomType || room.RoomType || '';
            const availabilityStatus = room.availabilityStatus || room.AvailabilityStatus || '';
            const tidyStatus = room.tidyStatus || room.TidyStatus || '';

            return `
                <div class="hk-modal-room-item" data-room-number="${(roomNumber || '').toString().toLowerCase()}" data-room-id="${roomId}">
                    <div class="form-check d-flex align-items-center">
                        <input class="form-check-input assign-room-checkbox" type="checkbox" value="${roomId}" id="assignRoom_${roomId}" data-room-number="${roomNumber}">
                        <label class="form-check-label ms-2 flex-grow-1 d-flex align-items-center flex-wrap gap-2" for="assignRoom_${roomId}">
                            <span class="fw-semibold">Room ${roomNumber}</span>
                            <span class="text-muted small">${roomType}</span>
                            <span class="hk-pill hk-pill-small room-status">${availabilityStatus}</span>
                            <span class="hk-pill hk-pill-small ${tidyStatus === 'Ready' ? 'success' : 'warning'}">${tidyStatus}</span>
                        </label>
                    </div>
                </div>
            `;
        }).join('');

        // Attach event listeners to checkboxes
        list.querySelectorAll('.assign-room-checkbox').forEach(cb => {
            cb.addEventListener('change', function () {
                const id = parseInt(this.value, 10);
                if (isNaN(id)) return;

                if (this.checked) {
                    state.modalSelected.add(id);
                } else {
                    state.modalSelected.delete(id);
                }
                syncModalSelectAll();
            });
        });

        state.modalFilteredRooms = rooms;
        filterModalRooms();
    }

    function filterModalRooms() {
        const query = normalize(document.getElementById('assignSearch')?.value || '');
        const items = document.querySelectorAll('#assignRoomList .hk-modal-room-item');
        let visible = 0;

        items.forEach(item => {
            const roomNumber = normalize(item.dataset.roomNumber || '');
            const checkbox = item.querySelector('.assign-room-checkbox');

            if (!query || roomNumber.includes(query)) {
                item.removeAttribute('hidden');
                if (checkbox) {
                    checkbox.removeAttribute('hidden');
                }
                visible++;
            } else {
                item.setAttribute('hidden', 'hidden');
                if (checkbox) {
                    checkbox.setAttribute('hidden', 'hidden');
                }
                // Uncheck hidden items
                if (checkbox && checkbox.checked) {
                    const id = parseInt(checkbox.value, 10);
                    state.modalSelected.delete(id);
                    checkbox.checked = false;
                }
            }
        });

        syncModalSelectAll();
        toggleEmptyState('assignRoomEmpty', visible === 0);
    }

    function toggleSelectAllModal(evt) {
        const checked = evt.target.checked;
        const visibleCheckboxes = Array.from(document.querySelectorAll('#assignRoomList .assign-room-checkbox'))
            .filter(cb => {
                const item = cb.closest('.hk-modal-room-item');
                return item && !item.hasAttribute('hidden');
            });

        visibleCheckboxes.forEach(cb => {
            cb.checked = checked;
            const id = parseInt(cb.value, 10);
            if (!isNaN(id)) {
                if (checked) {
                    state.modalSelected.add(id);
                } else {
                    state.modalSelected.delete(id);
                }
            }
        });

        syncModalSelectAll();
    }

    function syncModalSelectAll() {
        const selectAll = document.getElementById('assignSelectAll');
        if (!selectAll) return;

        // Only count visible checkboxes
        const visibleCheckboxes = Array.from(document.querySelectorAll('#assignRoomList .assign-room-checkbox'))
            .filter(cb => {
                const item = cb.closest('.hk-modal-room-item');
                return item && !item.hasAttribute('hidden');
            });

        if (visibleCheckboxes.length === 0) {
            selectAll.checked = false;
            selectAll.indeterminate = false;
            return;
        }

        const checkedCount = visibleCheckboxes.filter(cb => cb.checked).length;
        selectAll.indeterminate = checkedCount > 0 && checkedCount < visibleCheckboxes.length;
        selectAll.checked = checkedCount > 0 && checkedCount === visibleCheckboxes.length;
    }

    function submitAssignments() {
        if (!state.currentWorker) {
            showModalError('Please choose a worker.');
            return;
        }

        const roomIds = Array.from(state.modalSelected).filter(id => !isNaN(id) && id > 0);
        if (!roomIds.length) {
            showModalError('Please select at least one room to assign.');
            return;
        }

        showModalError(''); // Clear any previous errors
        toggleAssignButton(true);

        // Format workDate properly
        let workDateValue = state.workDate;
        if (typeof workDateValue === 'string') {
            if (!/^\d{4}-\d{2}-\d{2}$/.test(workDateValue)) {
                const date = new Date(workDateValue);
                if (!isNaN(date.getTime())) {
                    workDateValue = date.toISOString().split('T')[0];
                }
            }
        } else if (workDateValue instanceof Date) {
            workDateValue = workDateValue.toISOString().split('T')[0];
        }

        fetch(`${state.apiBase}/assign`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                workerId: state.currentWorker.id,
                workDate: workDateValue,
                roomIds: roomIds,
                notes: '',
                assignedBy: 'Housekeeping Dashboard'
            })
        })
            .then(async res => {
                if (!res.ok) {
                    const errorText = await res.text().catch(() => 'Unknown error');
                    throw new Error(errorText || `HTTP ${res.status}: Unable to assign rooms.`);
                }
                return res.json();
            })
            .then(data => {
                toggleAssignButton(false);
                if (data && data.success !== false) {
                    state.modalInstance.hide();
                    refreshDashboardData(state.workDate, { skipHistory: true });
                } else {
                    throw new Error(data.message || 'Assignment failed.');
                }
            })
            .catch(err => {
                console.error('Assignment error:', err);
                toggleAssignButton(false);
                showModalError(err.message || 'An error occurred while assigning rooms. Please try again.');
            });
    }

    function toggleAssignButton(isSubmitting) {
        const btn = document.getElementById('assignSubmitBtn');
        if (!btn) return;
        btn.disabled = isSubmitting;
        btn.querySelector('.spinner-border').classList.toggle('d-none', !isSubmitting);
        btn.querySelector('.default-label').classList.toggle('d-none', isSubmitting);
    }

    function showModalError(message) {
        const alert = document.getElementById('assignModalError');
        if (!alert) return;
        if (message) {
            alert.textContent = message;
            alert.classList.remove('d-none');
        } else {
            alert.textContent = '';
            alert.classList.add('d-none');
        }
    }

    function setModalLoading(isLoading) {
        const list = document.getElementById('assignRoomList');
        const empty = document.getElementById('assignRoomEmpty');
        if (!list) return;

        if (isLoading) {
            list.innerHTML = '<div class="text-center py-4"><div class="spinner-border text-primary" role="status" aria-hidden="true"></div><p class="mt-2 mb-0 small text-muted">Fetching available rooms...</p></div>';
            if (empty) empty.setAttribute('hidden', 'hidden');
        }
    }

    function clearModalSelections() {
        state.modalSelected.clear();
        showModalError('');
        const searchInput = document.getElementById('assignSearch');
        if (searchInput) searchInput.value = '';
        const selectAll = document.getElementById('assignSelectAll');
        if (selectAll) {
            selectAll.checked = false;
            selectAll.indeterminate = false;
        }
        toggleAssignButton(false);
    }

    function resetModalState() {
        clearModalSelections();
    }

    async function refreshDashboardData(workDate, options = {}) {
        if (!workDate || !state.apiBase) {
            return;
        }
        const { skipHistory = false, suppressErrors = false } = options;
        try {
            showRefreshing(true);
            const url = `${state.apiBase}/dashboard?workDate=${encodeURIComponent(workDate)}`;
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Failed to refresh dashboard (HTTP ${response.status})`);
            }
            const data = await response.json();
            state.workDate = (data.workDate || workDate).toString().substring(0, 10);
            state.unassignedRooms = data.unassignedRooms || [];
            state.matrixRows = data.assignmentMatrix || [];
            state.team = data.team || [];
            window.hkDashboardModel = data;

            updateDashboardStats(data);
            renderTeamCards(state.team);
            renderUnassignedCards(state.unassignedRooms);
            renderMatrixRows(state.matrixRows);
            updateDateControls(state.workDate);

            filterTeamCards();
            filterUnassignedCards();
            filterMatrixRows();

            if (!skipHistory) {
                updateBrowserQuery(state.workDate);
            }
        } catch (error) {
            console.error('Dashboard refresh failed', error);
            if (!suppressErrors) {
                alert('Unable to refresh dashboard. Please try again.');
            }
        } finally {
            showRefreshing(false);
        }
    }

    function updateDashboardStats(model) {
        const cleaned = document.getElementById('hkRoomsCleanedValue');
        const pending = document.getElementById('hkRoomsPendingValue');
        if (cleaned) {
            cleaned.textContent = `${model.roomsCompleted ?? 0} / ${model.totalRooms ?? 0}`;
        }
        if (pending) {
            pending.textContent = `${model.roomsPending ?? 0}`;
        }
    }

    function renderTeamCards(team) {
        const container = document.getElementById('hkTeamCards');
        if (!container) return;
        if (!Array.isArray(team) || !team.length) {
            container.innerHTML = '';
            toggleEmptyState('hkTeamEmpty', true);
            return;
        }
        toggleEmptyState('hkTeamEmpty', false);
        container.innerHTML = team.map(worker => {
            const name = escapeHtml(worker.workerName || '');
            const code = escapeHtml(worker.employeeCode || '');
            const dept = escapeHtml(worker.department || 'Housekeeping');
            const pending = worker.pendingRoomCount ?? 0;
            const assigned = worker.assignedRoomCount ?? 0;
            const workerId = worker.workerId ?? 0;
            return `
                <article class="hk-stack-card hk-worker-card" data-worker-name="${name}" data-department="${dept}" data-worker-id="${workerId}">
                    <div class="hk-stack-row">
                        <div>
                            <div class="hk-stack-title">${name}</div>
                            <div class="d-flex align-items-center gap-2 flex-wrap hk-stack-sub">
                                <span>Employee Code: ${code}</span>
                                <span class="hk-pill subtle mb-0">${dept}</span>
                            </div>
                            <div class="d-flex align-items-baseline gap-2 hk-pending-inline">
                                <span class="hk-label text-uppercase mb-0">Pending Rooms</span>
                                <strong class="hk-stack-value mb-0">${pending}</strong>
                            </div>
                        </div>
                        <div class="hk-stack-meta text-end">
                            <button type="button"
                                    class="btn btn-outline-primary btn-sm hk-open-assignment mt-2"
                                    data-worker-id="${workerId}"
                                    data-worker-name="${name}"
                                    data-worker-code="${code}">
                                ${assigned} Rooms Assigned
                            </button>
                        </div>
                    </div>
                </article>`;
        }).join('');
    }

    function renderUnassignedCards(rooms) {
        const container = document.getElementById('hkUnassignedCards');
        if (!container) return;
        if (!Array.isArray(rooms) || !rooms.length) {
            container.innerHTML = '';
            toggleEmptyState('hkUnassignedEmpty', true);
            return;
        }
        toggleEmptyState('hkUnassignedEmpty', false);
        container.innerHTML = rooms.map(room => {
            const number = escapeHtml(room.roomNumber || '');
            const type = escapeHtml(room.roomType || '');
            const availability = escapeHtml(room.availabilityStatus || 'Empty');
            const tidy = escapeHtml(room.tidyStatus || 'Untidy');
            const tidyClass = tidy === 'Ready' ? 'success' : 'warning';
            return `
                <article class="hk-stack-card hk-room-card"
                         data-room-number="${number.toLowerCase()}"
                         data-room-type="${type.toLowerCase()}"
                         data-availability="${availability.toLowerCase()}"
                         data-tidy="${tidy.toLowerCase()}">
                    <div class="hk-stack-row">
                        <div>
                            <div class="hk-stack-title">Room ${number}</div>
                            <div class="hk-stack-sub">${type}</div>
                        </div>
                        <div class="hk-stack-meta text-end">
                            <span class="hk-pill room-status">${availability}</span>
                            <span class="hk-pill ${tidyClass}">${tidy}</span>
                        </div>
                    </div>
                </article>`;
        }).join('');
    }

    async function refreshDashboardData(workDate, options = {}) {
        if (!workDate) {
            return;
        }
        state.workDate = workDate;
        showRefreshing(true);
        const query = encodeURIComponent(workDate);
        try {
            await Promise.all([
                loadPartial('#hkStatsContainer', `/Rooms/RoomAllocationStatsPartial?workDate=${query}`),
                loadPartial('#hkTeamCards', `/Rooms/RoomAllocationTeamPartial?workDate=${query}`),
                loadPartial('#hkUnassignedCards', `/Rooms/RoomAllocationUnassignedPartial?workDate=${query}`),
                loadPartial('#hkMatrixBody', `/Rooms/RoomAllocationMatrixPartial?workDate=${query}`)
            ]);
            updateDateControls(workDate);
            initFlatpickr({ reinitialize: true });
            filterTeamCards();
            filterUnassignedCards();
            filterMatrixRows();
        } catch (error) {
            if (!options.suppressErrors) {
                console.error('Dashboard refresh failed', error);
                alert('Unable to refresh dashboard. Please try again.');
            }
        } finally {
            showRefreshing(false);
        }
    }

    async function loadPartial(targetSelector, url) {
        const container = document.querySelector(targetSelector);
        if (!container) return;
        const response = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!response.ok) {
            throw new Error(`Failed to load partial: ${url}`);
        }
        const html = await response.text();
        container.innerHTML = html;
    }

    function updateDateControls(workDateIso) {
        if (!workDateIso) return;
        const display = document.getElementById('hkDateDisplay');
        const hidden = document.getElementById('hkWorkDate');
        const badge = document.getElementById('assignWorkDate');
        const formatted = formatDisplayDate(workDateIso);
        if (display) {
            display.value = formatted;
            if (display._flatpickr) {
                display._flatpickr.setDate(workDateIso, true);
            }
        }
        if (hidden) {
            hidden.value = workDateIso;
        }
        if (badge) {
            badge.textContent = formatted;
        }
    }

    function updateBrowserQuery(workDateIso) {
        try {
            const params = new URLSearchParams(window.location.search || '');
            params.set('workDate', workDateIso);
            const query = params.toString();
            const newUrl = `${window.location.pathname}?${query}`;
            window.history.replaceState({}, '', newUrl);
        } catch (err) {
            console.error('Failed to update browser history', err);
        }
    }

    function showRefreshing(isRefreshing) {
        document.body.classList.toggle('hk-refreshing', !!isRefreshing);
        const btn = document.getElementById('hkRefreshBtn');
        if (btn) {
            btn.disabled = !!isRefreshing;
        }
    }

    function formatDisplayDate(workDateIso) {
        if (!workDateIso) {
            return '';
        }
        const date = parseWorkDate(workDateIso);
        return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }).replace(/ /g, '-');
    }

    function toggleEmptyState(elementId, shouldShow) {
        const el = document.getElementById(elementId);
        if (!el) return;
        if (shouldShow) {
            el.removeAttribute('hidden');
        } else {
            el.setAttribute('hidden', 'hidden');
        }
    }

    function bindFilterControl(id, handler) {
        const el = document.getElementById(id);
        if (!el) return;
        const eventType = el.tagName === 'SELECT' ? 'change' : 'input';
        el.addEventListener(eventType, handler);
    }

    function parseWorkDate(value) {
        if (!value) {
            return new Date();
        }
        const normalized = value.toString().slice(0, 10);
        const isoMatch = normalized.match(/^(\d{4})-(\d{2})-(\d{2})$/);
        if (isoMatch) {
            const year = parseInt(isoMatch[1], 10);
            const month = parseInt(isoMatch[2], 10) - 1;
            const day = parseInt(isoMatch[3], 10);
            return new Date(year, month, day);
        }
        const parsed = new Date(value);
        if (!isNaN(parsed.getTime())) {
            return parsed;
        }
        return new Date();
    }

    function normalize(value) {
        return (value || '').toString().trim().toLowerCase();
    }
})();


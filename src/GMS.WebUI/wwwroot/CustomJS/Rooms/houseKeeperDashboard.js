(function () {
    const WORKER_MANUAL_WORK_DATE_KEY = 'hkWorkerManualWorkDate';
    let hasEnsuredDefault = false;
    let modalInitialized = false;
    let issueModalInitialized = false;
    const CLEAN_MODAL_MODES = {
        EDIT: 'edit',
        VIEW: 'view'
    };

    const cleanModalState = {
        checklist: [],
        checklistPromise: null,
        modalInstance: null,
        currentRoom: null,
        mode: CLEAN_MODAL_MODES.EDIT,
        selectedIds: [],
        reviewMeta: null
    };
    const issueModalState = {
        modalInstance: null,
        modalEl: null,
        currentRoom: null
    };

    document.addEventListener('DOMContentLoaded', initializeDashboard);

    function initializeDashboard() {
        const root = document.getElementById('hkWorkerRoot');
        if (!root) {
            return;
        }

        if (!hasEnsuredDefault) {
            hasEnsuredDefault = true;
            ensureClientDateDefault(root);
        }

        if (!modalInitialized) {
            initCleanModal();
            modalInitialized = true;
        }

        if (!issueModalInitialized) {
            initIssueModal();
            issueModalInitialized = true;
        }

        initFlatpickr(root);
        bindFilters(root);
        applyFilters(root);
        initActionButtons(root);
    }

    function configureModalForMode(modalEl, mode, options = {}) {
        const isView = mode === CLEAN_MODAL_MODES.VIEW;
        const selectAllWrapper = modalEl.querySelector('#hkCleanSelectAllWrapper');
        if (selectAllWrapper) {
            selectAllWrapper.classList.toggle('d-none', isView);
        }
        const selectAllInput = modalEl.querySelector('#hkCleanSelectAll');
        if (selectAllInput) {
            selectAllInput.checked = false;
            selectAllInput.disabled = isView;
        }
        const workDateWrapper = modalEl.querySelector('#hkCleanWorkDateWrapper');
        if (workDateWrapper) {
            workDateWrapper.classList.toggle('d-none', isView);
        }
        const reviewMeta = modalEl.querySelector('#hkCleanReviewMeta');
        if (reviewMeta) {
            reviewMeta.classList.toggle('d-none', !isView);
            const reviewedByLabel = modalEl.querySelector('#hkCleanReviewedBy');
            const reviewedOnLabel = modalEl.querySelector('#hkCleanReviewedOn');
            if (isView) {
                if (reviewedByLabel) {
                    reviewedByLabel.textContent = options.reviewMeta?.reviewedBy || 'Not available';
                }
                if (reviewedOnLabel) {
                    reviewedOnLabel.textContent = formatReviewedTimestamp(options.reviewMeta?.reviewedOn);
                }
            } else {
                if (reviewedByLabel) {
                    reviewedByLabel.textContent = '--';
                }
                if (reviewedOnLabel) {
                    reviewedOnLabel.textContent = '--';
                }
            }
        }
        const submitBtn = modalEl.querySelector('#hkCleanSubmit');
        if (submitBtn) {
            submitBtn.classList.toggle('d-none', isView);
        }
        const reasonField = modalEl.querySelector('#hkCleanReason');
        if (reasonField) {
            reasonField.readOnly = isView;
            reasonField.value = isView ? (options.reason || '') : '';
        }
        const commentsField = modalEl.querySelector('#hkCleanComments');
        if (commentsField) {
            commentsField.readOnly = isView;
            commentsField.value = isView ? (options.comments || '') : '';
        }
    }

    function formatReviewedTimestamp(value) {
        if (!value) {
            return 'Not available';
        }
        const date = typeof value === 'string' ? new Date(value) : value;
        if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
            return 'Not available';
        }
        return date.toLocaleString(undefined, {
            dateStyle: 'medium',
            timeStyle: 'short'
        });
    }

    function initFlatpickr(root) {
        if (!window.flatpickr) {
            return;
        }
        const input = root.querySelector('#hkDateDisplay');
        if (!input) {
            return;
        }
        if (input._flatpickr) {
            input._flatpickr.destroy();
        }
        flatpickr(input, {
            defaultDate: parseWorkDate(input.dataset.currentDate || input.value),
            dateFormat: 'd-M-Y',
            allowInput: false,
            onReady: (selectedDates, dateStr, instance) => {
                if (selectedDates && selectedDates[0]) {
                    instance.input.value = instance.formatDate(selectedDates[0], 'd-M-Y');
                } else {
                    const parsed = parseWorkDate(input.dataset.currentDate || input.value);
                    instance.setDate(parsed, true);
                }
            },
            onChange: (selectedDates, dateStr, instance) => {
                if (!selectedDates || !selectedDates.length) {
                    return;
                }
                const iso = instance.formatDate(selectedDates[0], 'Y-m-d');
                sessionStorage.setItem(WORKER_MANUAL_WORK_DATE_KEY, '1');
                input.dataset.currentDate = iso;
                input.value = instance.formatDate(selectedDates[0], 'd-M-Y');
                fetchAssignments(iso);
            }
        });
    }

    function fetchAssignments(workDate, options = {}) {
        const root = document.getElementById('hkWorkerRoot');
        if (!root) {
            return;
        }
        const fetchUrl = root.dataset.fetchUrl;
        if (!fetchUrl) {
            return;
        }

        root.classList.add('hk-loading');
        const url = new URL(fetchUrl, window.location.origin);
        if (workDate) {
            url.searchParams.set('workDate', workDate);
        }

        fetch(url.toString(), {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            credentials: 'same-origin'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to refresh assignments');
                }
                return response.text();
            })
            .then(html => {
                root.innerHTML = html;
                if (workDate) {
                    const dateInput = root.querySelector('#hkDateDisplay');
                    if (dateInput) {
                        dateInput.dataset.currentDate = workDate;
                        dateInput.value = formatDisplayDate(workDate);
                    }
                }
                if (window.hkWorkerDashboard) {
                    window.hkWorkerDashboard.workDate = workDate;
                }
                initializeDashboard();
            })
            .catch(() => {
                showToast('Unable to refresh assignments. Please try again.');
            })
            .finally(() => {
                root.classList.remove('hk-loading');
            });
    }

    function ensureClientDateDefault(root) {
        const manualSelection = sessionStorage.getItem(WORKER_MANUAL_WORK_DATE_KEY);
        if (manualSelection === '1') {
            return;
        }
        const input = root.querySelector('#hkDateDisplay');
        if (!input) {
            return;
        }
        const currentIso = input.dataset.currentDate || '';
        const todayIso = new Date().toISOString().split('T')[0];
        if (currentIso === todayIso) {
            sessionStorage.setItem(WORKER_MANUAL_WORK_DATE_KEY, '0');
            return;
        }

        sessionStorage.setItem(WORKER_MANUAL_WORK_DATE_KEY, '0');
        fetchAssignments(todayIso, { silent: true });
    }

    function bindFilters(root) {
        const search = root.querySelector('#hkAssignmentSearch');
        const status = root.querySelector('#hkStatusFilter');
        const occupancy = root.querySelector('#hkOccupancyFilter');

        if (search) {
            search.addEventListener('input', debounce(() => applyFilters(root), 150));
        }
        if (status) {
            status.addEventListener('change', () => applyFilters(root));
        }
        if (occupancy) {
            occupancy.addEventListener('change', () => applyFilters(root));
        }
    }

    function applyFilters(root) {
        const cards = root.querySelectorAll('[data-room-card]');
        if (!cards.length) {
            return;
        }

        const searchValue = (root.querySelector('#hkAssignmentSearch')?.value || '').trim().toLowerCase();
        const statusValue = (root.querySelector('#hkStatusFilter')?.value || '').trim().toLowerCase();
        const occupancyValue = (root.querySelector('#hkOccupancyFilter')?.value || '').trim().toLowerCase();
        let visible = 0;

        cards.forEach(card => {
            const roomNumber = (card.dataset.roomNumber || '').toLowerCase();
            const roomType = (card.dataset.roomType || '').toLowerCase();
            const status = (card.dataset.status || '').toLowerCase();
            const occupancy = (card.dataset.occupancy || '').toLowerCase();

            const matchesSearch = !searchValue || roomNumber.includes(searchValue) || roomType.includes(searchValue);
            const matchesStatus = !statusValue || status === statusValue;
            const matchesOccupancy = !occupancyValue || occupancy === occupancyValue;

            if (matchesSearch && matchesStatus && matchesOccupancy) {
                card.removeAttribute('hidden');
                visible++;
            } else {
                card.setAttribute('hidden', 'hidden');
            }
        });

        const emptyState = root.querySelector('#hkAssignmentsEmpty');
        if (emptyState) {
            emptyState.classList.toggle('d-none', visible > 0);
        }
    }

    function initActionButtons(root) {
        const cleanButtons = root.querySelectorAll('.hk-mark-clean-btn');
        cleanButtons.forEach(btn => {
            btn.addEventListener('click', event => {
                if (btn.disabled) {
                    return;
                }
                event.preventDefault();
                const roomId = parseInt(btn.dataset.roomId, 10);
                const roomNumber = btn.dataset.roomNumber || 'Room';
                if (!roomId) {
                    showToast('Room information missing.');
                    return;
                }
                handleMarkClean(roomId, roomNumber);
            });
        });

        const viewButtons = root.querySelectorAll('.hk-view-checklist-btn');
        viewButtons.forEach(btn => {
            btn.addEventListener('click', event => {
                event.preventDefault();
                const roomId = parseInt(btn.dataset.roomId, 10);
                const roomNumber = btn.dataset.roomNumber || 'Room';
                if (!roomId) {
                    showToast('Room information missing.');
                    return;
                }
                handleViewChecklist(roomId, roomNumber);
            });
        });

        const issueButtons = root.querySelectorAll('.hk-report-issue-btn');
        issueButtons.forEach(btn => {
            btn.addEventListener('click', event => {
                event.preventDefault();
                const roomId = parseInt(btn.dataset.roomId, 10);
                const roomNumber = btn.dataset.roomNumber || 'Room';
                if (!roomId) {
                    showToast('Room information missing.');
                    return;
                }
                openIssueModal(roomId, roomNumber);
            });
        });
    }

    function showToast(message) {
        const toast = document.getElementById('hkWorkerToast');
        if (!toast) {
            alert(message);
            return;
        }

        toast.textContent = message;
        toast.removeAttribute('hidden');
        toast.classList.add('active');

        if (toast.dataset.timeoutId) {
            clearTimeout(Number(toast.dataset.timeoutId));
        }
        const timeoutId = window.setTimeout(() => {
            toast.classList.remove('active');
            toast.setAttribute('hidden', 'hidden');
        }, 4000);
        toast.dataset.timeoutId = timeoutId;
    }

    function debounce(fn, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => fn.apply(this, args), delay);
        };
    }

    function formatDisplayDate(iso) {
        if (!window.flatpickr || !iso) {
            return iso;
        }
        return flatpickr.formatDate(parseWorkDate(iso), 'd-M-Y');
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

    function initCleanModal() {
        const modalEl = document.getElementById('hkCleanModal');
        if (!modalEl) {
            return;
        }
        const submitBtn = modalEl.querySelector('#hkCleanSubmit');
        if (submitBtn) {
            submitBtn.addEventListener('click', submitCleanChecklist);
        }
        const selectAll = modalEl.querySelector('#hkCleanSelectAll');
        if (selectAll) {
            selectAll.addEventListener('change', handleSelectAllToggle);
        }
        modalEl.addEventListener('hidden.bs.modal', () => {
            resetChecklistError();
            const selectAllCheck = modalEl.querySelector('#hkCleanSelectAll');
            if (selectAllCheck) {
                selectAllCheck.checked = false;
            }
            modalEl.querySelectorAll('input[name="hkCleanChecklistItem"]').forEach(input => {
                input.checked = false;
            });
            modalEl.querySelector('#hkCleanReason').value = '';
            modalEl.querySelector('#hkCleanComments').value = '';
            cleanModalState.currentRoom = null;
            cleanModalState.mode = CLEAN_MODAL_MODES.EDIT;
            cleanModalState.selectedIds = [];
            cleanModalState.reviewMeta = null;
            configureModalForMode(modalEl, CLEAN_MODAL_MODES.EDIT, {});
        });
    }

    function initIssueModal() {
        const modalEl = document.getElementById('hkIssueModal');
        if (!modalEl) {
            return;
        }
        issueModalState.modalEl = modalEl;
        const submitBtn = modalEl.querySelector('#hkIssueSubmit');
        if (submitBtn) {
            submitBtn.addEventListener('click', submitIssueReport);
        }
        modalEl.addEventListener('hidden.bs.modal', () => {
            const textarea = modalEl.querySelector('#hkIssueDetails');
            if (textarea) {
                textarea.value = '';
            }
            issueModalState.currentRoom = null;
            resetIssueError();
            setIssueButtonLoading(modalEl.querySelector('#hkIssueSubmit'), false);
        });
    }

    function ensureChecklist() {
        if (cleanModalState.checklist.length) {
            return Promise.resolve(cleanModalState.checklist);
        }
        if (cleanModalState.checklistPromise) {
            return cleanModalState.checklistPromise;
        }
        const apiBase = window.hkWorkerDashboard?.apiBase;
        if (!apiBase) {
            showToast('Checklist endpoint unavailable.');
            return Promise.reject();
        }
        const url = `${apiBase}/checklist`;
        cleanModalState.checklistPromise = fetch(url, {
            method: 'GET',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Unable to load checklist.');
                }
                return response.json();
            })
            .then(data => {
                cleanModalState.checklist = Array.isArray(data) ? data : [];
                return cleanModalState.checklist;
            })
            .catch(error => {
                showToast(error.message || 'Failed to load checklist.');
                throw error;
            })
            .finally(() => {
                cleanModalState.checklistPromise = null;
            });
        return cleanModalState.checklistPromise;
    }

    function handleMarkClean(roomId, roomNumber) {
        ensureChecklist()
            .then(checklist => {
                openCleanModal(roomId, roomNumber, checklist, { mode: CLEAN_MODAL_MODES.EDIT });
            })
            .catch(() => {});
    }

    function handleViewChecklist(roomId, roomNumber) {
        Promise.all([ensureChecklist(), fetchChecklistReview(roomId)])
            .then(([checklist, review]) => {
                if (!review) {
                    showToast('No saved checklist found for this room.');
                    return;
                }
                openCleanModal(roomId, roomNumber, checklist, {
                    mode: CLEAN_MODAL_MODES.VIEW,
                    selectedIds: review.checklistItemIds || [],
                    reason: review.reason || '',
                    comments: review.comments || '',
                    reviewMeta: review
                });
            })
            .catch(error => {
                if (error && error.message) {
                    showToast(error.message);
                }
            });
    }

    function fetchChecklistReview(roomId) {
        const apiBase = window.hkWorkerDashboard?.apiBase;
        if (!apiBase) {
            return Promise.reject(new Error('Checklist history unavailable.'));
        }
        const url = `${apiBase}/checklist/review?roomId=${encodeURIComponent(roomId)}`;
        return fetch(url, {
            method: 'GET',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json'
            }
        })
            .then(response => {
                if (response.status === 404) {
                    return null;
                }
                if (!response.ok) {
                    throw new Error('Unable to load checklist review.');
                }
                return response.json();
            });
    }

    function openCleanModal(roomId, roomNumber, checklist, options = {}) {
        const modalEl = document.getElementById('hkCleanModal');
        if (!modalEl) {
            return;
        }
        const mode = options.mode || CLEAN_MODAL_MODES.EDIT;
        const selectedIds = Array.isArray(options.selectedIds)
            ? options.selectedIds
                .map(id => parseInt(id, 10))
                .filter(id => !Number.isNaN(id))
            : [];
        cleanModalState.mode = mode;
        cleanModalState.selectedIds = selectedIds;
        cleanModalState.reviewMeta = options.reviewMeta || null;
        renderChecklistRows(checklist, { mode, selectedIds });
        const roomLabel = modalEl.querySelector('#hkCleanRoomLabel');
        if (roomLabel) {
            roomLabel.textContent = roomNumber;
        }
        const workDateLabel = modalEl.querySelector('#hkCleanWorkDate');
        if (workDateLabel && window.hkWorkerDashboard?.workDate) {
            workDateLabel.textContent = formatDisplayDate(window.hkWorkerDashboard.workDate);
        }
        resetChecklistError();
        cleanModalState.currentRoom = { roomId, roomNumber };
        configureModalForMode(modalEl, mode, options);

        if (!cleanModalState.modalInstance && window.bootstrap) {
            cleanModalState.modalInstance = new bootstrap.Modal(modalEl);
        }
        if (cleanModalState.modalInstance) {
            cleanModalState.modalInstance.show();
        } else {
            modalEl.classList.add('show');
            modalEl.style.display = 'block';
        }
    }

    function openIssueModal(roomId, roomNumber) {
        const modalEl = issueModalState.modalEl || document.getElementById('hkIssueModal');
        if (!modalEl) {
            showToast('Issue form unavailable.');
            return;
        }
        issueModalState.modalEl = modalEl;
        const roomLabel = modalEl.querySelector('#hkIssueRoomLabel');
        if (roomLabel) {
            roomLabel.textContent = roomNumber;
        }
        const textarea = modalEl.querySelector('#hkIssueDetails');
        if (textarea) {
            textarea.value = '';
        }
        resetIssueError();
        issueModalState.currentRoom = { roomId, roomNumber };

        if (!issueModalState.modalInstance && window.bootstrap) {
            issueModalState.modalInstance = new bootstrap.Modal(modalEl);
        }
        if (issueModalState.modalInstance) {
            issueModalState.modalInstance.show();
        } else {
            modalEl.classList.add('show');
            modalEl.style.display = 'block';
        }
    }

    function renderChecklistRows(checklist, config = {}) {
        const body = document.getElementById('hkCleanChecklistBody');
        if (!body) {
            return;
        }
        if (!Array.isArray(checklist) || !checklist.length) {
            body.innerHTML = '<tr><td colspan="2" class="text-center text-muted py-4">No checklist items configured.</td></tr>';
            return;
        }
        const mode = config.mode || CLEAN_MODAL_MODES.EDIT;
        const selectedSet = new Set(
            Array.isArray(config.selectedIds)
                ? config.selectedIds
                    .map(id => parseInt(id, 10))
                    .filter(id => !Number.isNaN(id))
                : []
        );
        body.innerHTML = checklist.map((item, index) => {
            const rawId = item.id ?? item.ID ?? item.Id ?? index;
            const label = item.chklist ?? item.Chklist ?? item.Checklist ?? 'Checklist item';
            const description = item.description ?? item.Description ?? '';
            const value = Number.isFinite(Number(rawId)) ? Number(rawId) : rawId;
            const isChecked = selectedSet.has(value);
            const statusCell = mode === CLEAN_MODAL_MODES.VIEW
                ? `<i class="bi ${isChecked ? 'bi-check-circle-fill text-success' : 'bi-x-circle-fill text-danger'} hk-clean-checklist-icon"></i>`
                : `<input class="form-check-input" type="checkbox" name="hkCleanChecklistItem" value="${value}" ${isChecked ? 'checked' : ''}>`;
            return `
            <tr>
                <td>
                    ${statusCell}
                </td>
                <td>
                    <div class="fw-semibold">${label}</div>
                    ${description ? `<div class="text-muted small">${description}</div>` : ''}
                </td>
            </tr>
        `;
        }).join('');
    }

    function handleSelectAllToggle(event) {
        if (cleanModalState.mode !== CLEAN_MODAL_MODES.EDIT) {
            event.target.checked = false;
            return;
        }
        const isChecked = event.target.checked;
        document.querySelectorAll('#hkCleanChecklistBody input[name="hkCleanChecklistItem"]').forEach(input => {
            input.checked = isChecked;
        });
    }

    function submitIssueReport() {
        const modalEl = issueModalState.modalEl || document.getElementById('hkIssueModal');
        if (!modalEl || !issueModalState.currentRoom) {
            return;
        }
        const textarea = modalEl.querySelector('#hkIssueDetails');
        const notes = (textarea?.value || '').trim();
        if (!notes) {
            showIssueError('Please describe the issue.');
            return;
        }
        const apiBase = window.hkWorkerDashboard?.apiBase;
        if (!apiBase) {
            showIssueError('Unable to reach housekeeping service.');
            return;
        }
        const workDate = window.hkWorkerDashboard?.workDate || new Date().toISOString().split('T')[0];
        const payload = {
            roomId: issueModalState.currentRoom.roomId,
            workDate,
            notes,
            reportedBy: null
        };
        const submitBtn = modalEl.querySelector('#hkIssueSubmit');
        setIssueButtonLoading(submitBtn, true);
        resetIssueError();
        fetch(`${apiBase}/report-issue`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(payload)
        })
            .then(response => {
                if (response.ok) {
                    return null;
                }
                return response
                    .json()
                    .catch(() => response.text().then(text => ({ message: text })))
                    .then(errorPayload => {
                        const message = (errorPayload && (errorPayload.message || errorPayload.Message)) || 'Failed to report issue.';
                        throw new Error(message);
                    });
            })
            .then(() => {
                showToast('Issue reported successfully.');
                if (issueModalState.modalInstance) {
                    issueModalState.modalInstance.hide();
                } else {
                    modalEl.classList.remove('show');
                    modalEl.style.display = 'none';
                }
                fetchAssignments(workDate);
            })
            .catch(error => {
                showIssueError(error.message || 'Unable to report issue right now.');
            })
            .finally(() => {
                setIssueButtonLoading(submitBtn, false);
            });
    }

    function submitCleanChecklist() {
        const modalEl = document.getElementById('hkCleanModal');
        if (!modalEl || !cleanModalState.currentRoom) {
            return;
        }
        if (cleanModalState.mode !== CLEAN_MODAL_MODES.EDIT) {
            return;
        }
        const roomId = cleanModalState.currentRoom.roomId;
        const workDate = window.hkWorkerDashboard?.workDate || new Date().toISOString().split('T')[0];
        const checklistIds = collectSelectedChecklistIds();
        if (!checklistIds.length) {
            showChecklistError('Please select at least one action item.');
            return;
        }
        const payload = {
            roomId,
            workDate,
            checklistItemIds: checklistIds,
            reason: modalEl.querySelector('#hkCleanReason').value.trim(),
            comments: modalEl.querySelector('#hkCleanComments').value.trim()
        };
        const apiBase = window.hkWorkerDashboard?.apiBase;
        if (!apiBase) {
            showChecklistError('Unable to reach housekeeping service.');
            return;
        }
        const submitBtn = modalEl.querySelector('#hkCleanSubmit');
        setButtonLoading(submitBtn, true);
        resetChecklistError();
        fetch(`${apiBase}/mark-clean`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify(payload)
        })
            .then(response => {
                if (response.ok) {
                    return null;
                }
                return response
                    .json()
                    .catch(() => response.text().then(text => ({ message: text })))
                    .then(errorPayload => {
                        const message = (errorPayload && (errorPayload.message || errorPayload.Message)) || 'Failed to mark as clean.';
                        throw new Error(message);
                    });
            })
            .then(() => {
                showToast('Room marked as cleaned.');
                if (cleanModalState.modalInstance) {
                    cleanModalState.modalInstance.hide();
                } else {
                    modalEl.classList.remove('show');
                    modalEl.style.display = 'none';
                }
                fetchAssignments(workDate);
            })
            .catch(error => {
                showChecklistError(error.message || 'Unable to complete cleaning checklist.');
            })
            .finally(() => {
                setButtonLoading(submitBtn, false);
            });
    }

    function collectSelectedChecklistIds() {
        const inputs = document.querySelectorAll('#hkCleanChecklistBody input[name="hkCleanChecklistItem"]:checked');
        return Array.from(inputs)
            .map(input => parseInt(input.value, 10))
            .filter(id => !Number.isNaN(id));
    }

    function setButtonLoading(button, isLoading) {
        if (!button) {
            return;
        }
        button.disabled = isLoading;
        const spinner = button.querySelector('.spinner-border');
        const label = button.querySelector('.default-label');
        if (spinner) {
            spinner.classList.toggle('d-none', !isLoading);
        }
        if (label) {
            label.classList.toggle('d-none', isLoading);
        }
    }

    function showChecklistError(message) {
        const alert = document.getElementById('hkCleanChecklistError');
        if (!alert) {
            showToast(message);
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }

    function resetChecklistError() {
        const alert = document.getElementById('hkCleanChecklistError');
        if (alert) {
            alert.classList.add('d-none');
            alert.textContent = '';
        }
    }

    function showIssueError(message) {
        const alert = document.getElementById('hkIssueError');
        if (!alert) {
            showToast(message);
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }

    function resetIssueError() {
        const alert = document.getElementById('hkIssueError');
        if (alert) {
            alert.classList.add('d-none');
            alert.textContent = '';
        }
    }

    function setIssueButtonLoading(button, isLoading) {
        if (!button) {
            return;
        }
        button.disabled = isLoading;
        const spinner = button.querySelector('.spinner-border');
        const label = button.querySelector('.default-label');
        if (spinner) {
            spinner.classList.toggle('d-none', !isLoading);
        }
        if (label) {
            label.classList.toggle('d-none', isLoading);
        }
    }
})();


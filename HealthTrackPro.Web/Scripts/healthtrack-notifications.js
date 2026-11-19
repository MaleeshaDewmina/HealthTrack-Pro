/* HealthTrack Pro - local notification dropdown; depends on the existing jQuery bundle. */
(function () {
    'use strict';
    var root = document.getElementById('htNotificationDropdown');
    if (!root || !window.jQuery) { return; }

    var $ = window.jQuery;
    var bell = document.getElementById('htnfBell');
    var panel = document.getElementById('htnfPanel');
    var close = document.getElementById('htnfClose');
    var badge = document.getElementById('htnfBadge');
    var count = document.getElementById('htnfCount');
    var list = document.getElementById('htnfList');
    var markAll = document.getElementById('htnfMarkAll');
    var token = root.querySelector('input[name="__RequestVerificationToken"]');
    var refreshing = false;
    var busy = false;

    function make(tag, cls, value) {
        var node = document.createElement(tag);
        if (cls) { node.className = cls; }
        if (value !== undefined) { node.textContent = value; }
        return node;
    }
    function niceDate(value) {
        var dt = new Date(value);
        if (isNaN(dt.getTime())) { return ''; }
        return dt.toLocaleString(undefined, {
            year: 'numeric', month: 'short', day: 'numeric',
            hour: 'numeric', minute: '2-digit'
        });
    }
    function showPlaceholder(msg) {
        list.replaceChildren(make('p', 'htnf-placeholder', msg));
    }
    function render(data) {
        var unread = Math.max(0, parseInt(data.unreadCount, 10) || 0);
        badge.hidden = unread === 0;
        badge.textContent = unread > 99 ? '99+' : String(unread);
        count.textContent = unread === 1 ? '1 unread' : (unread + ' unread');
        bell.setAttribute('aria-label', unread ? ('Notifications, ' + unread + ' unread') : 'Notifications');
        markAll.disabled = unread === 0;

        var items = data.items || [];
        if (!items.length) { showPlaceholder("You're all caught up. No notifications yet."); return; }
        list.textContent = '';
        items.forEach(function (item) {
            var wrapper = make('article', 'htnf-item ' + (item.isRead ? 'read' : 'unread'));
            wrapper.appendChild(make('span', 'htnf-dot'));
            var main = make('div', 'htnf-main');
            var title = make('div', 'htnf-item-title');
            title.appendChild(make('strong', '', item.title || 'Notification'));
            if (!item.isRead) { title.appendChild(make('span', 'htnf-new', 'New')); }
            main.appendChild(title);
            main.appendChild(make('p', '', item.message || ''));
            main.appendChild(make('div', 'htnf-meta', (item.type ? item.type + ' · ' : '') + niceDate(item.createdAtUtc)));
            wrapper.appendChild(main);
            if (!item.isRead) {
                var readBtn = make('button', 'htnf-read', '✓');
                readBtn.type = 'button';
                readBtn.title = 'Mark as read';
                readBtn.setAttribute('aria-label', 'Mark ' + (item.title || 'notification') + ' as read');
                readBtn.addEventListener('click', function () { change('read', item.id); });
                wrapper.appendChild(readBtn);
            }
            list.appendChild(wrapper);
        });
    }
    function refresh() {
        if (refreshing) { return; }
        refreshing = true;
        return $.ajax({ url: root.getAttribute('data-recent-url'), method: 'GET', dataType: 'json', cache: false })
            .done(render)
            .fail(function () { if (!panel.hidden) { showPlaceholder('Could not load notifications. Use View all notifications instead.'); } })
            .always(function () { refreshing = false; });
    }
    function change(type, id) {
        if (busy || !token) { return; }
        busy = true;
        markAll.disabled = true;
        var formData = { __RequestVerificationToken: token.value };
        if (type === 'read') { formData.id = id; }
        $.ajax({
            url: root.getAttribute(type === 'read' ? 'data-read-url' : 'data-readall-url'),
            method: 'POST', dataType: 'json', data: formData
        }).done(function () { refresh(); })
          .fail(function () { showPlaceholder('Unable to update notifications. Please try again.'); })
          .always(function () { busy = false; });
    }
    function toggle(open) {
        panel.hidden = !open;
        bell.setAttribute('aria-expanded', open ? 'true' : 'false');
        if (open) { refresh(); }
    }

    bell.addEventListener('click', function () { toggle(panel.hidden); });
    close.addEventListener('click', function () { toggle(false); bell.focus(); });
    markAll.addEventListener('click', function () { change('all'); });
    document.addEventListener('click', function (e) { if (!root.contains(e.target) && !panel.hidden) { toggle(false); } });
    document.addEventListener('keydown', function (e) { if (e.key === 'Escape' && !panel.hidden) { toggle(false); bell.focus(); } });
    window.setInterval(function () { if (!document.hidden) { refresh(); } }, 60000);
    document.addEventListener('visibilitychange', function () { if (!document.hidden) { refresh(); } });
    refresh();
})();

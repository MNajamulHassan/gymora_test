/* ── Apply theme before page paint to avoid flash ── */
(function () {
  var saved = localStorage.getItem('gymora-theme') || 'dark';
  document.documentElement.setAttribute('data-theme', saved);

  /* CRITICAL: kill sidebar margin on mobile immediately */
  if (window.innerWidth < 992) {
    document.addEventListener('DOMContentLoaded', function () {
      var mainEl = document.getElementById('gymoraMain');
      if (mainEl) {
        mainEl.style.setProperty('margin-left', '0', 'important');
        mainEl.style.setProperty('width', '100%', 'important');
      }
    });
  }
})();

/* ============================================================
   Gymora — Main JS
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

  /* ── 1. Theme toggle ── */
  var themeToggle = document.getElementById('themeToggle');
  var themeIcon   = document.getElementById('themeIcon');

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('gymora-theme', theme);
    if (themeIcon) {
      themeIcon.className = theme === 'light' ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
    }
  }

  applyTheme(localStorage.getItem('gymora-theme') || 'dark');

  if (themeToggle) {
    themeToggle.addEventListener('click', function () {
      var current = document.documentElement.getAttribute('data-theme');
      applyTheme(current === 'light' ? 'dark' : 'light');
    });
  }

  /* ── 2. User dropdown ── */
  var trigger = document.getElementById('userDropdownTrigger');
  var menu    = document.getElementById('userDropdownMenu');
  var wrapper = document.getElementById('userDropdownWrapper');

  if (trigger && menu) {
    trigger.addEventListener('click', function (e) {
      e.stopPropagation();
      var open = menu.classList.toggle('open');
      trigger.classList.toggle('open', open);
      trigger.setAttribute('aria-expanded', String(open));
    });
    document.addEventListener('click', function (e) {
      if (wrapper && !wrapper.contains(e.target)) {
        menu.classList.remove('open');
        trigger.classList.remove('open');
        trigger.setAttribute('aria-expanded', 'false');
      }
    });
    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') {
        menu.classList.remove('open');
        trigger.classList.remove('open');
        trigger.setAttribute('aria-expanded', 'false');
      }
    });
  }

  /* ── 3. Mobile: force margin-left = 0 always ── */
  function fixMobileLayout() {
    var mainEl = document.getElementById('gymoraMain');
    if (!mainEl) return;
    if (window.innerWidth < 992) {
      mainEl.style.setProperty('margin-left', '0', 'important');
      mainEl.style.setProperty('width', '100%', 'important');
    }
  }

  fixMobileLayout();
  window.addEventListener('resize', fixMobileLayout);

  /* ── 4. Desktop sidebar collapse ── */
  if (window.innerWidth >= 992) {
    var sidebar     = document.getElementById('gymSidebar');
    var mainEl      = document.getElementById('gymoraMain');
    var mainContent = document.getElementById('mainContent');
    if (!sidebar || !mainEl) return;

    var collapseTimer  = null;
    var isHoverSidebar = false;
    var isCollapsed    = false;

    function setCollapsed(val) {
      if (window.innerWidth < 992) return;
      isCollapsed = val;
      if (val) {
        sidebar.classList.add('collapsed');
        mainEl.style.marginLeft = '64px';
      } else {
        sidebar.classList.remove('collapsed');
        mainEl.style.marginLeft = '240px';
      }
    }

    function scheduleCollapse() {
      if (collapseTimer) clearTimeout(collapseTimer);
      collapseTimer = setTimeout(function () {
        if (!isHoverSidebar) setCollapsed(true);
      }, 800);
    }

    mainEl.addEventListener('click', function (e) {
      if (!sidebar.contains(e.target)) scheduleCollapse();
    });
    window.addEventListener('scroll', scheduleCollapse, { passive: true });
    if (mainContent) {
      mainContent.addEventListener('scroll', scheduleCollapse, { passive: true });
      mainContent.addEventListener('click', scheduleCollapse);
    }
    sidebar.addEventListener('mouseenter', function () {
      isHoverSidebar = true;
      if (collapseTimer) clearTimeout(collapseTimer);
      setCollapsed(false);
    });
    sidebar.addEventListener('mouseleave', function () {
      isHoverSidebar = false;
      scheduleCollapse();
    });
    document.addEventListener('mousemove', function (e) {
      if (e.clientX <= 10 && isCollapsed) {
        isHoverSidebar = true;
        setCollapsed(false);
      }
    });
    window.addEventListener('resize', function () {
      if (window.innerWidth < 992) {
        mainEl.style.setProperty('margin-left', '0', 'important');
        sidebar.classList.remove('collapsed');
      }
    });
  }

  /* ── 5. Auto-dismiss success alerts after 5s ── */
  document.querySelectorAll('.alert-success, .alert-info').forEach(function (alertEl) {
    setTimeout(function () {
      try { bootstrap.Alert.getOrCreateInstance(alertEl).close(); } catch(e) {}
    }, 5000);
  });

});

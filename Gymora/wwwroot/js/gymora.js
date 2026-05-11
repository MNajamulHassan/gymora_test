/* Apply saved theme before first paint to avoid flash */
(function () {
  var saved = localStorage.getItem('gymora-theme') || 'dark';
  document.documentElement.setAttribute('data-theme', saved);

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
  var themeLabel  = document.getElementById('themeLabel');

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('gymora-theme', theme);

    var isDark = theme === 'dark';

    if (themeIcon) {
      themeIcon.className = isDark ? 'bi bi-moon-fill' : 'bi bi-sun-fill';
    }
    if (themeLabel) {
      themeLabel.textContent = isDark ? 'Dark Mode' : 'Light Mode';
    }
  }

  applyTheme(localStorage.getItem('gymora-theme') || 'dark');

  if (themeToggle) {
    themeToggle.addEventListener('click', function (e) {
      e.stopPropagation(); /* keep dropdown open */
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

  /* ── 4. Desktop sidebar: auto-collapse on content interaction ──
     - Collapses (icons-only, 64px) 800ms after user clicks/scrolls main area
     - Instantly expands on sidebar hover or mouse touching left edge          */
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

    /* Collapse when user interacts with main content */
    mainEl.addEventListener('click', function (e) {
      if (!sidebar.contains(e.target)) scheduleCollapse();
    });
    window.addEventListener('scroll', scheduleCollapse, { passive: true });
    if (mainContent) {
      mainContent.addEventListener('scroll', scheduleCollapse, { passive: true });
      mainContent.addEventListener('click', scheduleCollapse);
    }

    /* Expand when hovering over the sidebar */
    sidebar.addEventListener('mouseenter', function () {
      isHoverSidebar = true;
      if (collapseTimer) clearTimeout(collapseTimer);
      setCollapsed(false);
    });
    sidebar.addEventListener('mouseleave', function () {
      isHoverSidebar = false;
      scheduleCollapse();
    });

    /* Edge-peek: move mouse to far left to expand */
    document.addEventListener('mousemove', function (e) {
      if (e.clientX <= 10 && isCollapsed) {
        isHoverSidebar = true;
        setCollapsed(false);
      }
    });

    /* Keep layout correct on window resize */
    window.addEventListener('resize', function () {
      if (window.innerWidth < 992) {
        mainEl.style.setProperty('margin-left', '0', 'important');
        sidebar.classList.remove('collapsed');
        isCollapsed = false;
      } else if (!isCollapsed) {
        mainEl.style.marginLeft = '240px';
      }
    });
  }

  /* ── 5. Auto-dismiss success alerts after 5s ── */
  document.querySelectorAll('.alert-success, .alert-info').forEach(function (alertEl) {
    setTimeout(function () {
      try { bootstrap.Alert.getOrCreateInstance(alertEl).close(); } catch (e) { /* ignore */ }
    }, 5000);
  });

});

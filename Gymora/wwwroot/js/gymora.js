(function () {
  var saved = localStorage.getItem('gymora-theme')
              || 'dark';
  document.documentElement
          .setAttribute('data-theme', saved);
})();

// ============================================================
// Gymora — Vanilla JavaScript (no jQuery)
// ============================================================

document.addEventListener('DOMContentLoaded', function () {

  /* ── Theme toggle ── */
  var themeToggle = document.getElementById('themeToggle');
  var themeIcon   = document.getElementById('themeIcon');

  function applyTheme(theme) {
    document.documentElement
            .setAttribute('data-theme', theme);
    localStorage.setItem('gymora-theme', theme);
    if (themeIcon) {
      themeIcon.className = theme === 'light'
        ? 'bi bi-sun-fill'
        : 'bi bi-moon-fill';
    }
  }

  applyTheme(
    localStorage.getItem('gymora-theme') || 'dark'
  );

  if (themeToggle) {
    themeToggle.addEventListener('click', function () {
      var current = document.documentElement
                            .getAttribute('data-theme');
      applyTheme(current === 'light' ? 'dark' : 'light');
    });
  }
  /* ── End theme toggle ── */

    // 1. Auto-dismiss Bootstrap success/info alerts after 5 seconds
    var autoDismissAlerts = document.querySelectorAll('.alert-success, .alert-info');
    autoDismissAlerts.forEach(function (alertEl) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alertEl);
            bsAlert.close();
        }, 5000);
    });

    // 2. Highlight current page nav link
    var currentPath = window.location.pathname.toLowerCase();
    var navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach(function (link) {
        var href = link.getAttribute('href');
        if (href && href !== '/' && href !== '#') {
            if (currentPath.startsWith(href.toLowerCase())) {
                link.classList.add('active-page');
            }
        }
    });

});

// ============================================================
// Stage 6 — Sidebar, mobile nav, path-based active state
// ============================================================

(function () {
    'use strict';

    var MOBILE_MQ = '(max-width: 768px)';
    var SCROLL_COMPACT_THRESHOLD = 40;
    var DRAG_TAB_PX = 55;
    var SCROLL_IDLE_MS = 2000;

    function gymoraCanonicalPath(pathname) {
        if (!pathname) return '/';
        var p = pathname.toLowerCase();
        if (p.length > 1 && p.slice(-1) === '/') p = p.slice(0, -1);
        return p || '/';
    }

    function gymoraLinkPath(anchor) {
        var h = anchor.getAttribute('href');
        if (!h || h === '#' || h.indexOf('javascript:') === 0) return null;
        try {
            var u = new URL(h, window.location.origin);
            return gymoraCanonicalPath(u.pathname);
        } catch (e) {
            return null;
        }
    }

    function gymoraBestMatchingAnchor(anchors, currentPath) {
        var best = null;
        var bestLen = -1;
        for (var i = 0; i < anchors.length; i++) {
            var a = anchors[i];
            if (a.classList.contains('disabled')) continue;
            var lp = gymoraLinkPath(a);
            if (!lp) continue;
            var match = currentPath === lp || (currentPath.indexOf(lp + '/') === 0 && lp !== '/');
            if (match && lp.length > bestLen) {
                bestLen = lp.length;
                best = a;
            }
        }
        return best;
    }

    function gymoraGetMobileTabs(navEl) {
        var bubble = document.getElementById('mobileBubble');
        var items = navEl ? navEl.querySelectorAll('.gymora-nav-mobile-item') : [];
        var list = [];
        if (bubble) list.push(bubble);
        for (var i = 0; i < items.length; i++) list.push(items[i]);
        return list;
    }

    function gymoraPositionBubble(navEl, activeEl) {
        var bubble = document.getElementById('mobileBubble');
        if (!navEl || !bubble || !activeEl) return;
        var navRect = navEl.getBoundingClientRect();
        var elRect = activeEl.getBoundingClientRect();
        var left = elRect.left - navRect.left + elRect.width / 2 - bubble.offsetWidth / 2;
        var maxL = Math.max(0, navEl.offsetWidth - bubble.offsetWidth);
        left = Math.max(0, Math.min(left, maxL));
        bubble.style.left = left + 'px';
        bubble.style.right = 'auto';
    }

    function gymoraClearSidebarActive() {
        document.querySelectorAll('.gymora-sidebar .gymora-nav-link.active').forEach(function (el) {
            el.classList.remove('active');
        });
    }

    function gymoraClearMobileActive(navEl) {
        if (!navEl) return;
        navEl.querySelectorAll('.gymora-nav-mobile-item.active').forEach(function (el) {
            el.classList.remove('active');
        });
        var b = document.getElementById('mobileBubble');
        if (b) b.classList.remove('active');
    }

    function gymoraHighlightByPathname() {
        var path = gymoraCanonicalPath(window.location.pathname);
        var navEl = document.getElementById('mobileNav');

        gymoraClearSidebarActive();
        gymoraClearMobileActive(navEl);

        var sidebarLinks = document.querySelectorAll('.gymora-sidebar a.gymora-nav-link[href]');
        var sb = gymoraBestMatchingAnchor(sidebarLinks, path);
        if (sb) sb.classList.add('active');

        if (navEl) {
            var mobileAnchors = navEl.querySelectorAll('a.gymora-nav-mobile-item[href], a#mobileBubble[href], a.gymora-mobile-bubble[href]');
            var uniq = [];
            var seen = {};
            for (var i = 0; i < mobileAnchors.length; i++) {
                var a = mobileAnchors[i];
                if (!seen[a]) {
                    seen[a] = true;
                    uniq.push(a);
                }
            }
            var mb = gymoraBestMatchingAnchor(uniq, path);
            if (mb) {
                mb.classList.add('active');
                gymoraPositionBubble(navEl, mb);
            } else {
                var bubble = document.getElementById('mobileBubble');
                if (bubble) gymoraPositionBubble(navEl, bubble);
            }
        }
    }

    function gymoraEnsureOverlay() {
        var o = document.getElementById('gymora-sidebar-overlay');
        if (o) return o;
        o = document.createElement('div');
        o.id = 'gymora-sidebar-overlay';
        o.setAttribute('aria-hidden', 'true');
        o.style.cssText = 'display:none;position:fixed;inset:0;background:rgba(0,0,0,0.55);z-index:99;';
        document.body.appendChild(o);
        return o;
    }

    function gymoraIsMobileSidebarContext() {
        return window.matchMedia(MOBILE_MQ).matches;
    }

    function gymoraCloseSidebar(sidebar, overlay) {
        if (!sidebar) return;
        sidebar.classList.remove('mobile-open');
        if (overlay) overlay.style.display = 'none';
        if (gymoraIsMobileSidebarContext()) sidebar.style.display = '';
    }

    function gymoraToggleSidebar(sidebar, overlay) {
        if (!sidebar) return;
        var open = !sidebar.classList.contains('mobile-open');
        if (open) {
            if (gymoraIsMobileSidebarContext()) sidebar.style.display = 'flex';
            sidebar.classList.add('mobile-open');
            if (overlay) overlay.style.display = 'block';
        } else {
            gymoraCloseSidebar(sidebar, overlay);
        }
    }

    function gymoraInitSidebarToggle() {
        var toggle = document.getElementById('sidebarToggle');
        var sidebar = document.querySelector('.gymora-sidebar');
        if (!toggle || !sidebar) return;
        var overlay = gymoraEnsureOverlay();
        toggle.addEventListener('click', function () {
            gymoraToggleSidebar(sidebar, overlay);
        });
        overlay.addEventListener('click', function () {
            gymoraCloseSidebar(sidebar, overlay);
        });
        window.addEventListener('resize', function () {
            if (!gymoraIsMobileSidebarContext()) {
                gymoraCloseSidebar(sidebar, overlay);
            }
        });
    }

    function gymoraInitMobileNavScroll() {
        var mobileNav = document.getElementById('mobileNav');
        var mainContent = document.getElementById('mainContent');
        if (!mobileNav) return;

        mobileNav.classList.add('expanded');
        mobileNav.classList.remove('compact');

        var lastScrollY = window.scrollY || document.documentElement.scrollTop || 0;
        var scrollIdleTimer = null;

        function maxScrollTop() {
            var w = window.scrollY || document.documentElement.scrollTop || 0;
            var m = mainContent ? mainContent.scrollTop : 0;
            return Math.max(w, m);
        }

        function onScroll() {
            var y = maxScrollTop();
            var delta = y - lastScrollY;
            lastScrollY = y;

            if (scrollIdleTimer) clearTimeout(scrollIdleTimer);

            if (y > SCROLL_COMPACT_THRESHOLD) {
                if (delta > 2) {
                    mobileNav.classList.add('compact');
                    mobileNav.classList.remove('expanded');
                } else if (delta < -2) {
                    mobileNav.classList.remove('compact');
                    mobileNav.classList.add('expanded');
                }
            } else {
                mobileNav.classList.remove('compact');
                mobileNav.classList.add('expanded');
            }

            scrollIdleTimer = setTimeout(function () {
                mobileNav.classList.remove('compact');
                mobileNav.classList.add('expanded');
            }, SCROLL_IDLE_MS);
        }

        window.addEventListener('scroll', onScroll, { passive: true });
        if (mainContent) mainContent.addEventListener('scroll', onScroll, { passive: true });
    }

    function gymoraInitMobileNavDrag() {
        var navEl = document.getElementById('mobileNav');
        if (!navEl) return;

        var tabs = gymoraGetMobileTabs(navEl);
        if (tabs.length === 0) return;

        var touchStartX = 0;
        var indexAtStart = 0;

        function indexOfActive() {
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].classList.contains('active')) return i;
            }
            return 0;
        }

        navEl.addEventListener('touchstart', function (e) {
            if (!e.touches || e.touches.length === 0) return;
            touchStartX = e.touches[0].clientX;
            indexAtStart = indexOfActive();
        }, { passive: true });

        navEl.addEventListener('touchmove', function (e) {
            if (!e.touches || e.touches.length === 0) return;
            var x = e.touches[0].clientX;
            var delta = x - touchStartX;
            var stepDelta = Math.trunc(delta / DRAG_TAB_PX);
            var newIdx = indexAtStart + stepDelta;
            newIdx = Math.max(0, Math.min(newIdx, tabs.length - 1));
            gymoraClearMobileActive(navEl);
            tabs[newIdx].classList.add('active');
            gymoraPositionBubble(navEl, tabs[newIdx]);
        }, { passive: true });

        navEl.addEventListener('touchend', function () {
            var endIdx = indexOfActive();
            if (endIdx !== indexAtStart) {
                var el = tabs[endIdx];
                if (gymoraLinkPath(el) && !el.classList.contains('disabled')) {
                    var h = el.getAttribute('href');
                    if (h && h !== '#') window.location.href = h;
                }
            }
        });
    }

    function gymoraInitMobileNavTapExpand() {
        var navEl = document.getElementById('mobileNav');
        if (!navEl) return;

        navEl.addEventListener('click', function (e) {
            if (!navEl.classList.contains('compact')) return;
            e.preventDefault();
            e.stopPropagation();
            navEl.classList.remove('compact');
            navEl.classList.add('expanded');
            var a = e.target.closest('a');
            if (a && navEl.contains(a) && gymoraLinkPath(a) && !a.classList.contains('disabled')) {
                var h = a.getAttribute('href');
                if (h && h !== '#') {
                    setTimeout(function () {
                        window.location.href = h;
                    }, 0);
                }
            }
        }, true);
    }

    function gymoraInit() {
        gymoraInitSidebarToggle();
        gymoraInitMobileNavScroll();
        gymoraHighlightByPathname();
        gymoraInitMobileNavDrag();
        gymoraInitMobileNavTapExpand();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', gymoraInit);
    } else {
        gymoraInit();
    }
})();

// @ts-check
const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

const BASE_URL = 'http://localhost:4200';
const OUT_DIR = path.join(__dirname, '..', 'docs', 'screenshots');

if (!fs.existsSync(OUT_DIR)) fs.mkdirSync(OUT_DIR, { recursive: true });

const PAGES = [
  { url: '/auth/login',   file: 'login.png',            wait: false },
  { url: '/dashboard',    file: 'salle-controle.png',   wait: true  },
  { url: '/call-center',  file: 'salle-appels.png',     wait: true  },
  { url: '/patients',     file: 'dossiers-patients.png',wait: true  },
  { url: '/encounters',   file: 'consultations.png',    wait: true  },
  { url: '/billing',      file: 'salle-factures.png',   wait: true  },
  { url: '/ai',           file: 'assistant-ia.png',     wait: true  },
  { url: '/audit',        file: 'journal-audit.png',    wait: true  },
  { url: '/admin',        file: 'acces-restreint.png',  wait: true  },
  { url: '/admin/roles',  file: 'roles-permissions.png',wait: true  },
];

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
  const page = await context.newPage();

  // --- Log in ---
  await page.goto(`${BASE_URL}/auth/login`, { waitUntil: 'networkidle' });
  await page.fill('input[type="email"]', 'admin@cleanink.fr');
  await page.fill('input[type="password"]', 'password123');
  await page.click('button[type="submit"]');
  // Wait for redirect away from login
  await page.waitForURL(url => !url.href.includes('/auth/login'), { timeout: 8000 }).catch(() => {});

  // Inject a fake token in localStorage so the guard passes even if API is down
  await page.evaluate(() => {
    localStorage.setItem('token', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZXYiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjk5OTk5OTk5OTl9.fake');
  });

  // Screenshot login separately (without auth)
  const loginCtx = await browser.newContext({ viewport: { width: 1440, height: 900 } });
  const loginPage = await loginCtx.newPage();
  await loginPage.goto(`${BASE_URL}/auth/login`, { waitUntil: 'networkidle' });
  await loginPage.waitForTimeout(800);
  await loginPage.screenshot({ path: path.join(OUT_DIR, 'login.png'), fullPage: false });
  console.log('✓ login.png');
  await loginCtx.close();

  // Screenshot all authenticated pages
  for (const { url, file, wait } of PAGES) {
    if (file === 'login.png') continue; // already done
    await page.goto(`${BASE_URL}${url}`, { waitUntil: 'domcontentloaded' });
    if (wait) await page.waitForTimeout(1200);
    await page.screenshot({ path: path.join(OUT_DIR, file), fullPage: false });
    console.log(`✓ ${file}`);
  }

  await browser.close();
  console.log('\nDone — screenshots saved to docs/screenshots/');
})();

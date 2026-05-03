import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, HeaderComponent, SidebarComponent],
  template: `
    <div class="fn-layout" [class.fn-layout--collapsed]="sidebarCollapsed">

      <!-- Deep background fractal -->
      <div class="fn-layout__bg">
        <div class="fn-layout__bg-glow fn-layout__bg-glow--1"></div>
        <div class="fn-layout__bg-glow fn-layout__bg-glow--2"></div>
        <div class="fn-layout__bg-grid"></div>
      </div>

      <app-sidebar
        [collapsed]="sidebarCollapsed"
        (toggleCollapse)="sidebarCollapsed = !sidebarCollapsed"
      ></app-sidebar>

      <div class="fn-layout__main">
        <app-header
          (toggleSidebar)="sidebarCollapsed = !sidebarCollapsed"
        ></app-header>

        <main class="fn-layout__content">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `,
  styles: [`
    .fn-layout {
      display: flex;
      height: 100vh;
      overflow: hidden;
      background: var(--fn-abyss);
      position: relative;
    }

    /* Fractal ambient backgrounds */
    .fn-layout__bg {
      position: fixed;
      inset: 0;
      pointer-events: none;
      z-index: 0;
    }
    .fn-layout__bg-glow {
      position: absolute;
      border-radius: 50%;
      filter: blur(80px);
    }
    .fn-layout__bg-glow--1 {
      width: 600px; height: 400px;
      top: -100px; right: -100px;
      background: radial-gradient(ellipse, rgba(20,40,100,.35) 0%, transparent 70%);
      animation: fn-pulse-vein 8s ease-in-out infinite;
    }
    .fn-layout__bg-glow--2 {
      width: 400px; height: 300px;
      bottom: -80px; left: 200px;
      background: radial-gradient(ellipse, rgba(61,232,176,.04) 0%, transparent 70%);
      animation: fn-pulse-vein 12s ease-in-out infinite reverse;
    }
    .fn-layout__bg-grid {
      position: absolute;
      inset: 0;
      background-image:
        linear-gradient(rgba(28,47,90,.08) 1px, transparent 1px),
        linear-gradient(90deg, rgba(28,47,90,.08) 1px, transparent 1px);
      background-size: 40px 40px;
      mask-image: radial-gradient(ellipse 80% 80% at 50% 50%, black 30%, transparent 100%);
    }

    .fn-layout__main {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
      position: relative;
      z-index: 1;
    }

    .fn-layout__content {
      flex: 1;
      overflow-y: auto;
      padding: 1.75rem 2rem;
    }

    @media (max-width: 768px) {
      .fn-layout__content { padding: 1rem; }
    }
  `],
})
export class LayoutComponent {
  sidebarCollapsed = false;
}

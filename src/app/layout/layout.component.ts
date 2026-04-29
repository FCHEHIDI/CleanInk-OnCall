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
    <div class="ci-layout" [class.sidebar-collapsed]="sidebarCollapsed">
      <app-sidebar
        [collapsed]="sidebarCollapsed"
        (toggleCollapse)="sidebarCollapsed = !sidebarCollapsed"
      ></app-sidebar>

      <div class="ci-layout__main">
        <app-header
          (toggleSidebar)="sidebarCollapsed = !sidebarCollapsed"
        ></app-header>

        <main class="ci-layout__content">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `,
  styles: [`
    .ci-layout {
      display: flex;
      height: 100vh;
      overflow: hidden;
      background: var(--color-bg);
    }

    .ci-layout__main {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
      transition: margin-left 0.25s ease;
    }

    .ci-layout__content {
      flex: 1;
      overflow-y: auto;
      padding: 1.5rem 2rem;
    }

    @media (max-width: 768px) {
      .ci-layout__content {
        padding: 1rem;
      }
    }
  `],
})
export class LayoutComponent {
  sidebarCollapsed = false;
}

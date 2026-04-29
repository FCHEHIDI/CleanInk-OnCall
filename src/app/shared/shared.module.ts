import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';

// Shared Components
import { StatusBadgeComponent } from './components/status-badge/status-badge.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';

const MAT_MODULES = [
  MatButtonModule,
  MatCardModule,
  MatIconModule,
  MatInputModule,
  MatFormFieldModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatDialogModule,
  MatSnackBarModule,
  MatProgressSpinnerModule,
  MatChipsModule,
  MatBadgeModule,
  MatTooltipModule,
  MatMenuModule,
  MatSelectModule,
  MatDatepickerModule,
];

const SHARED_COMPONENTS = [
  StatusBadgeComponent,
  PageHeaderComponent,
  LoadingSpinnerComponent,
  EmptyStateComponent,
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    ...MAT_MODULES,
    ...SHARED_COMPONENTS,
  ],
  exports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    ...MAT_MODULES,
    ...SHARED_COMPONENTS,
  ],
})
export class SharedModule {}

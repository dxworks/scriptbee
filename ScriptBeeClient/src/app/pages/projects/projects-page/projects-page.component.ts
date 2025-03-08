import { Component, effect, viewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ProjectService } from '../../../services/projects/project.service';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { Project } from '../../../types/project';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { createRxResourceHandler } from '../../../utils/resource';

@Component({
  selector: 'app-projects-page',
  templateUrl: './projects-page.component.html',
  styleUrls: ['./projects-page.component.scss'],
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    LoadingProgressBarComponent,
    MatIcon,
    MatButton,
    DatePipe,
    RouterLink,
    ErrorStateComponent,
  ],
})
export class ProjectsPage {
  displayedColumns: string[] = ['id', 'name', 'creationDate'];
  dataSource: MatTableDataSource<Project> = new MatTableDataSource<Project>();

  paginator = viewChild.required(MatPaginator);
  sort = viewChild.required(MatSort);

  getAllProjectsResource = createRxResourceHandler({
    loader: () => this.projectsService.getAllProjects(),
  });

  constructor(
    private projectsService: ProjectService,
    private router: Router
  ) {
    effect(() => {
      this.dataSource.data = this.getAllProjectsResource.value() || [];
    });

    effect(() => {
      this.dataSource.paginator = this.paginator();
      this.dataSource.sort = this.sort();
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  onRowClick(row: Project) {
    this.router.navigate([`/projects/${row.id}`]).then();
  }
}

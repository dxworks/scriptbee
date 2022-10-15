import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Project, ProjectData } from '../state/project-details/project';
import { ProjectService } from '../services/project/project.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateProjectDialogComponent } from './create-project-dialog/create-project-dialog.component';
import { DeleteProjectDialogComponent } from './delete-project-dialog/delete-project-dialog.component';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent {
  displayedColumns: string[] = ['projectId', 'projectName', 'creationDate', 'deleteProject'];
  dataSource?: MatTableDataSource<ProjectData>;

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  constructor(public dialog: MatDialog, private projectService: ProjectService, private router: Router, private snackBar: MatSnackBar) {
    this.getAllProjects();
  }

  applyFilter(event: Event) {
    if (!this.dataSource) {
      return;
    }

    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  onCreateButtonClick(event: Event) {
    const dialogRef = this.dialog.open(CreateProjectDialogComponent, {
      width: '300px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getAllProjects();
      }
    }, (error: any) => {
      this.snackBar.open('Could not create project!', 'Ok', {
        duration: 4000
      });
    });
  }

  onDeleteButtonClick(event: Event, row: Project) {
    event.stopPropagation();
    const dialogRef = this.dialog.open(DeleteProjectDialogComponent, {
      width: '300px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.projectService.deleteProject(row.data.projectId).subscribe(res => {
          this.getAllProjects();
        });
      }
    }, (error: any) => {
      this.snackBar.open('Could not delete project!', 'Ok', {
        duration: 4000
      });
    });
  }

  private getAllProjects() {
    this.projectService.getAllProjects().subscribe(projects => {
      // todo
      this.dataSource = new MatTableDataSource(projects);
      this.dataSource.paginator = this.paginator ?? null;
      this.dataSource.sort = this.sort ?? null;
    });
  }

  onRowClick(row: Project) {
    this.router.navigate([`/projects/${row.data.projectId}`]).then(r => {
    });
  }
}

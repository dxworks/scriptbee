import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ProjectService} from '../services/project/project.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MatDialog} from '@angular/material/dialog';
import {LoaderService} from '../services/loader/loader.service';
import {ProjectDetailsService} from './project-details.service';
import {UploadService} from '../services/upload/upload.service';
import {LinkerService} from '../services/linker/linker.service';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss']
})
export class ProjectDetailsComponent implements OnInit {

  constructor(public dialog: MatDialog, private projectService: ProjectService, private loaderService: LoaderService,
              private uploadService: UploadService, private route: ActivatedRoute, private snackBar: MatSnackBar,
              private projectDetailsService: ProjectDetailsService, private linkerService: LinkerService) {
  }

  ngOnInit(): void {
    this.projectDetailsService.clearData();

    const id = this.route.snapshot.paramMap.get('id');

    this.projectService.getProject(id).subscribe(result => {
      if (result) {
        this.projectDetailsService.project.next(result);
      }
    }, () => {
      this.snackBar.open('Could not get project!', 'Ok', {
        duration: 4000
      });
    });

    this.loaderService.getAllLoaders().subscribe(result => {
      if (result) {
        this.projectDetailsService.loaders.next(result);
      }
    }, () => {
      this.snackBar.open('Could not get loaders!', 'Ok', {
        duration: 4000
      });
    });

    this.linkerService.getAllLinkers().subscribe({
      next: (result) => {
        this.projectDetailsService.linkers.next(result);
      },
      error: () => {
        this.snackBar.open('Could not get linkers!', 'Ok', {
          duration: 4000
        });
      }
    })

    this.projectService.getProjectContext(id).subscribe(result => {
      this.projectDetailsService.context.next(result);
    });
  }
}

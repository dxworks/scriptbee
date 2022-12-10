import { Component, Input, OnInit } from '@angular/core';
import { Project } from "../../../state/project-details/project";
import { Store } from "@ngrx/store";
import { ErrorDialogService } from "../../../shared/error-dialog/error-dialog.service";
import { ProjectService } from "../../../services/project/project.service";
import { LinkerService } from "../../../services/linker/linker.service";
import { MatSnackBar } from "@angular/material/snack-bar";

@Component({
  selector: 'app-link-models',
  templateUrl: './link-models.component.html',
  styleUrls: ['./link-models.component.scss']
})
export class LinkModelsComponent implements OnInit {
  @Input()
  project: Project | undefined;

  linkers: string[] = [];
  selectedLinker: string | undefined;
  linking = false;

  constructor(private store: Store, private errorDialogService: ErrorDialogService,
    private linkerService: LinkerService, private projectService: ProjectService, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.linkerService.getAllLinkers().subscribe({
      next: (res) => {
        this.linkers = res;
      },
      error: () => {
        this.snackBar.open('Could not get linkers!', 'Ok', {
          duration: 4000
        });
      }
    });
  }

  onLinkButtonClick() {
    if (this.selectedLinker) {
      this.linking = true;
      const projectId = this.project.data.projectId;

      // todo
      this.linkerService.linkModels(projectId, this.selectedLinker).subscribe({
        next: () => {
          this.projectService.getProjectContext(projectId).subscribe({
            next: (res) => {
              this.linking = false;
            },
            error: (error) => {
              this.linking = false;
              this.errorDialogService.displayDialogErrorMessage('Could not get project context', LinkModelsComponent.getErrorMessage(error));
            }
          });
        },
        error: (error) => {
          this.linking = false;
          this.errorDialogService.displayDialogErrorMessage('Could not link project context', LinkModelsComponent.getErrorMessage(error));
        }
      });
    } else {
      this.linking = false;
      this.errorDialogService.displayDialogErrorMessage('You must select a linker first', '');
    }
  }

  // todo after errors are standardized, remove this
  private static getErrorMessage(error: any) {
    return error.error?.detail ?? error.error;
  }
}

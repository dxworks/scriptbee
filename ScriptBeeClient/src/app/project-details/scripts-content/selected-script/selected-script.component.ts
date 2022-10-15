import { Component, OnDestroy, OnInit } from '@angular/core';
import { ThemeService } from '../../../services/theme/theme.service';
import { FileSystemService } from '../../../services/file-system/file-system.service';
import { ProjectDetailsService } from '../../project-details.service';
import { ActivatedRoute } from '@angular/router';
import { RunScriptService } from '../../../services/run-script/run-script.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Store } from '@ngrx/store';
import { NotificationsService } from "../../../services/notifications/notifications.service";

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss']
})
export class SelectedScriptComponent implements OnInit, OnDestroy {

  editorOptions = {theme: 'vs-dark', language: 'javascript', readOnly: true, automaticLayout: true};
  code = '';
  scriptPath = '';
  scriptAbsolutePath = '';
  projectAbsolutePath = '';
  isLoadingResults: boolean = false;

  constructor(private themeService: ThemeService, private fileSystemService: FileSystemService,
              private projectDetailsService: ProjectDetailsService, private route: ActivatedRoute,
              private runScriptService: RunScriptService, private snackBar: MatSnackBar,
              private store: Store, private notificationService: NotificationsService) {
  }

  ngOnDestroy(): void {
  }

  ngOnInit(): void {

    this.notificationService.watchedFiles
      .subscribe({
        next: watchedFile => {
          if (this.scriptPath === watchedFile.path) {
            this.code = watchedFile.content;
          }
        }
      });

    this.route.params.subscribe(params => {
      if (params) {
        this.scriptPath = params['scriptPath'];
        this.setEditorLanguage(this.scriptPath);

        // todo
        // this.projectDetailsService.project.subscribe(project => {
        //   if (project) {
        //     this.fileSystemService.getScriptAbsolutePath(project.data.projectId, this.scriptPath).subscribe(absolutePath => {
        //       this.scriptAbsolutePath = absolutePath;
        //     });
        //
        //     this.fileSystemService.getProjectAbsolutePath(project.data.projectId).subscribe(projectPath => {
        //       this.projectAbsolutePath = projectPath;
        //     });
        //
        //     this.fileSystemService.getFileContent(project.data.projectId, this.scriptPath).subscribe(content => {
        //       this.code = content;
        //     });
        //
        //     this.fileSystemService.postFileWatcher(project.data.projectId, this.scriptPath)
        //       .subscribe();
        //   }
        // }, (error: any) => {
        //   this.snackBar.open('Could not get project!', 'Ok', {
        //     duration: 4000
        //   });
        // });
      }
    });

    this.themeService.darkThemeSubject.subscribe(isDark => {
      if (isDark) {
        this.editorOptions = {...this.editorOptions, theme: 'vs-dark'};
      } else {
        this.editorOptions = {...this.editorOptions, theme: 'vs-light'};
      }
    });
  }

  onRunScriptButtonClick() {
    // this.projectDetailsService.lastRunErrorMessage.next('');
    this.isLoadingResults = true;

    // todo refactor to use rxjs not nested subscribes
    // this.projectDetailsService.project.subscribe(project => {
    //   if (project) {
    //     // todo remove hardcoded values
    //     // todo change subscribe signature
    //     this.runScriptService.runScriptFromPath(project.data.projectId, this.scriptPath, this.getLanguage(this.scriptPath)).subscribe({
    //       next: (result) => {
    //         if (result) {
    //           console.log(result)
    //
    //           result.results.forEach(r => {
    //             this.store.dispatch(setOutput({
    //               outputId: r.outputId,
    //               projectId: result.projectId,
    //               outputType: r.outputType,
    //               path: r.path,
    //               loading: false
    //             }))
    //           });
    //
    //           this.isLoadingResults = false;
    //           this.projectDetailsService.lastRunResult.next(result);
    //           this.projectDetailsService.lastRunErrorMessage.next('');
    //         }
    //       },
    //       error: (err: any) => {
    //         this.isLoadingResults = false;
    //         if (err instanceof HttpErrorResponse) {
    //           this.projectDetailsService.lastRunErrorMessage.next(err.error.detail);
    //         }
    //         this.snackBar.open('Could not run script!', 'Ok', {
    //           duration: 4000
    //         });
    //       }
    //     });
    //   }
    // });
  }

  onCopyToClipboardButtonClick() {
    this.snackBar.open('Path copied successfully!', 'Ok', {
      duration: 2000
    });
  }

  private setEditorLanguage(scriptPath: string) {
    if (!scriptPath) {
      return;
    }

    if (scriptPath.endsWith('.js')) {
      this.editorOptions = {...this.editorOptions, language: 'javascript'};
    } else if (scriptPath.endsWith('.py')) {
      this.editorOptions = {...this.editorOptions, language: 'python'};
    } else if (scriptPath.endsWith('.cs')) {
      this.editorOptions = {...this.editorOptions, language: 'csharp'};
    }
  }

  private createInjector() {
    // return Injector.create({})
  }

  // todo refactor (maybe move in webapp)
  private getLanguage(filename: string) {
    if (!filename) {
      return "";
    }

    if (filename.endsWith('.js')) {
      return 'javascript';
    } else if (filename.endsWith('.py')) {
      return 'python';
    } else if (filename.endsWith('.cs')) {
      return 'csharp';
    }

    return "";
  }
}

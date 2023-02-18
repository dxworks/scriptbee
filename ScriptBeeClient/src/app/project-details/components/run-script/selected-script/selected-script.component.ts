import { Component, OnInit } from '@angular/core';
import { ThemeService } from '../../../../services/theme/theme.service';
import { FileSystemService } from '../../../../services/file-system/file-system.service';
import { ActivatedRoute } from '@angular/router';
import { RunScriptService } from '../../../../services/run-script/run-script.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Store } from '@ngrx/store';
import { NotificationsService } from '../../../../services/notifications/notifications.service';
import { selectScriptTreeLeafClick } from '../../../../state/script-tree/script-tree.selectors';
import { filter, map, switchMap } from 'rxjs/operators';
import { forkJoin, of } from 'rxjs';
import { selectProjectDetails } from '../../../../state/project-details/project-details.selectors';
import { Project } from '../../../../state/project-details/project';
import { setOutput } from '../../../../state/outputs/output.actions';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss']
})
export class SelectedScriptComponent implements OnInit {
  editorOptions = {
    theme: 'vs-dark',
    language: 'javascript',
    readOnly: true,
    automaticLayout: true
  };
  code = '';
  scriptPath = '';
  scriptAbsolutePath = '';
  projectAbsolutePath = '';
  isLoadingResults = false;
  project: Project | undefined;

  constructor(
    private themeService: ThemeService,
    private fileSystemService: FileSystemService,
    private route: ActivatedRoute,
    private runScriptService: RunScriptService,
    private snackBar: MatSnackBar,
    private store: Store,
    private notificationService: NotificationsService
  ) {}

  ngOnInit(): void {
    this.notificationService.watchedFiles.subscribe({
      next: (watchedFile) => {
        if (this.scriptPath === watchedFile.path) {
          this.code = watchedFile.content;
        }
      }
    });

    this.store
      .select(selectProjectDetails)
      .pipe(
        filter((x) => !!x),
        switchMap((project) => {
          return this.store.select(selectScriptTreeLeafClick).pipe(
            map((leaf) => leaf?.filePath ?? this.route.snapshot.paramMap.get('scriptPath')),
            switchMap((filePath) =>
              forkJoin([
                of(project),
                of(filePath),
                this.fileSystemService.getProjectAbsolutePath(project.data.projectId),
                this.fileSystemService.getScriptAbsolutePath(project.data.projectId, filePath),
                this.fileSystemService.getFileContent(project.data.projectId, filePath)
              ])
            )
          );
        })
      )
      .subscribe(([project, filePath, projectPath, absolutePath, content]) => {
        this.project = project;
        this.scriptPath = filePath;
        this.projectAbsolutePath = projectPath;
        this.scriptAbsolutePath = absolutePath;
        this.code = content;
        this.setEditorLanguage(this.scriptPath);
      });

    this.themeService.darkThemeSubject.subscribe((isDark) => {
      if (isDark) {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-dark' };
      } else {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-light' };
      }
    });
  }

  onRunScriptButtonClick() {
    if (!this.project) {
      return;
    }

    this.isLoadingResults = true;

    this.runScriptService.runScriptFromPath(this.project.data.projectId, this.scriptPath, this.getLanguage(this.scriptPath)).subscribe({
      next: (run) => {
        this.isLoadingResults = false;
        this.store.dispatch(
          setOutput({
            runIndex: run.index,
            scriptName: run.scriptName,
            results: run.results
          })
        );
      },
      error: () => {
        // todo handle error and display it in the output errors tab
        this.isLoadingResults = false;

        this.snackBar.open('Could not run script!', 'Ok', {
          duration: 4000
        });
      }
    });
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
      this.editorOptions = { ...this.editorOptions, language: 'javascript' };
    } else if (scriptPath.endsWith('.py')) {
      this.editorOptions = { ...this.editorOptions, language: 'python' };
    } else if (scriptPath.endsWith('.cs')) {
      this.editorOptions = { ...this.editorOptions, language: 'csharp' };
    }
  }

  private createInjector() {
    // return Injector.create({})
  }

  // todo refactor (maybe move in webapp)
  private getLanguage(filename: string) {
    if (!filename) {
      return '';
    }

    if (filename.endsWith('.js')) {
      return 'javascript';
    } else if (filename.endsWith('.py')) {
      return 'python';
    } else if (filename.endsWith('.cs')) {
      return 'csharp';
    }

    return '';
  }
}

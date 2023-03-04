import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../../../../services/theme/theme.service';
import {FileSystemService} from '../../../../services/file-system/file-system.service';
import {ActivatedRoute} from '@angular/router';
import {RunScriptService} from '../../../../services/run-script/run-script.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Store} from '@ngrx/store';
import {NotificationsService} from '../../../../services/notifications/notifications.service';
import {selectScriptTreeLeafClick} from '../../../../state/script-tree/script-tree.selectors';
import {filter, map, switchMap} from 'rxjs/operators';
import {forkJoin, of} from 'rxjs';
import {selectProjectDetails} from '../../../../state/project-details/project-details.selectors';
import {Project} from '../../../../state/project-details/project';
import {setOutput} from '../../../../state/outputs/output.actions';
import {ScriptsStore} from '../../../stores/scripts-store.service';
import {ScriptLanguage} from '../../../services/script-types';
import {MatDialog} from '@angular/material/dialog';
import {EditParametersDialogComponent} from '../edit-parameters-dialog/edit-parameters-dialog.component';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss'],
  providers: [ScriptsStore],
})
export class SelectedScriptComponent implements OnInit {
  editorOptions = {
    theme: 'vs-dark',
    language: 'csharp',
    readOnly: true,
    automaticLayout: true,
  };
  code = '';
  scriptPath = '';
  projectAbsolutePath = '';
  isLoadingResults = false;
  project: Project | undefined;

  scriptContentError$ = this.scriptsStore.scriptContentError;
  scriptContentLoading$ = this.scriptsStore.scriptContentLoading;

  private availableScriptLanguages: ScriptLanguage[] = [];

  constructor(
    private themeService: ThemeService,
    private fileSystemService: FileSystemService,
    private route: ActivatedRoute,
    private runScriptService: RunScriptService,
    private snackBar: MatSnackBar,
    private store: Store,
    private scriptsStore: ScriptsStore,
    private notificationService: NotificationsService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.notificationService.watchedFiles.subscribe({
      next: (watchedFile) => {
        if (this.scriptPath === watchedFile.path) {
          this.code = watchedFile.content;
        }
      },
    });

    const scriptPath = this.route.snapshot.paramMap.get('scriptPath');
    const projectId = this.project?.data.projectId;
    if (scriptPath && projectId) {
      this.scriptsStore.loadScriptContent({
        scriptId: scriptPath,
        projectId: projectId,
      });
    }

    this.route.paramMap.subscribe({
      next: (params) => {
        const scriptPath = params.get('scriptPath');
        const projectId = this.project?.data.projectId;
        if (scriptPath && projectId) {
          this.scriptsStore.loadScriptContent({
            scriptId: scriptPath,
            projectId: projectId,
          });
        }
      },
    });

    this.scriptsStore.scriptContent.subscribe({
      next: (scriptContent) => {
        this.code = scriptContent;
      },
    });

    this.store
      .select(selectProjectDetails)
      .pipe(
        filter((x) => !!x),
        switchMap((project) => {
          return this.store.select(selectScriptTreeLeafClick).pipe(
            map((leaf) => leaf?.filePath ?? this.route.snapshot.paramMap.get('scriptPath')),
            switchMap((filePath) => forkJoin([of(project), of(filePath), this.fileSystemService.getProjectAbsolutePath(project.data.projectId)]))
          );
        })
      )
      .subscribe(([project, filePath, projectAbsolutePath]) => {
        this.project = project;
        this.scriptPath = filePath;
        this.projectAbsolutePath = projectAbsolutePath;
        this.setEditorLanguage(this.scriptPath);
      });

    this.themeService.darkThemeSubject.subscribe((isDark) => {
      if (isDark) {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-dark' };
      } else {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-light' };
      }
    });

    this.scriptsStore.loadAvailableLanguages();
    this.scriptsStore.availableLanguages.subscribe((languages) => {
      return (this.availableScriptLanguages = languages);
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
            results: run.results,
          })
        );
      },
      error: () => {
        // todo handle error and display it in the output errors tab
        this.isLoadingResults = false;

        this.snackBar.open('Could not run script!', 'Ok', {
          duration: 4000,
        });
      },
    });
  }

  onCopyToClipboardButtonClick() {
    this.snackBar.open('Path copied successfully!', 'Ok', {
      duration: 2000,
    });
  }

  private setEditorLanguage(scriptPath: string) {
    if (!scriptPath) {
      return;
    }

    for (const language of this.availableScriptLanguages) {
      if (scriptPath.endsWith(language.extension)) {
        this.editorOptions = { ...this.editorOptions, language: language.name };
        return;
      }
    }
  }

  private createInjector() {
    // return Injector.create({})
  }

  private getLanguage(filename: string) {
    if (!filename) {
      return '';
    }

    for (const language of this.availableScriptLanguages) {
      if (filename.endsWith(language.extension)) {
        return language.name;
      }
    }

    return '';
  }

  onEditParametersButtonClick() {
    this.dialog.open(EditParametersDialogComponent, {
      disableClose: true,
      data: {
        scriptId: this.scriptPath,
        projectId: this.project.data.projectId,
        parameters: [],
      },
    });
  }
}

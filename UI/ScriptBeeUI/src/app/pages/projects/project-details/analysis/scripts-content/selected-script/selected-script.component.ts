import { Component, OnInit } from '@angular/core';
import { ThemeService } from '../../../../services/theme/theme.service';
import { FileSystemService } from '../../../../services/file-system/file-system.service';
import { ActivatedRoute } from '@angular/router';
import { RunScriptService } from '../../../../services/run-script/run-script.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Store } from '@ngrx/store';
import { NotificationsService } from '../../../../services/notifications/notifications.service';
import { setOutput } from '../../../../state/outputs/output.actions';
import { ScriptsStore } from '../../../stores/scripts-store.service';
import { ScriptLanguage } from '../../../services/script-types';
import { MatDialog } from '@angular/material/dialog';
import { EditParametersDialogComponent } from '../edit-parameters-dialog/edit-parameters-dialog.component';
import { ProjectStore } from '../../../stores/project-store.service';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss'],
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
  projectId = '';
  projectAbsolutePath = '';
  isLoadingResults = false;
  scriptContentError$ = this.scriptsStore.scriptContentError;
  scriptContentLoading$ = this.scriptsStore.scriptContentLoading;

  private availableScriptLanguages: ScriptLanguage[] = [];

  constructor(
    private projectStore: ProjectStore,
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
    this.projectId = this.projectStore.getProjectId();

    this.notificationService.watchedFiles.subscribe({
      next: (watchedFile) => {
        if (this.scriptPath === watchedFile.path) {
          this.code = watchedFile.content;
        }
      },
    });

    this.route.paramMap.subscribe({
      next: (params) => {
        const scriptPath = params.get('scriptPath');
        this.scriptPath = scriptPath ?? '';

        if (scriptPath && this.projectId) {
          this.setEditorLanguage(scriptPath);

          this.scriptsStore.loadScriptContent({
            scriptId: scriptPath,
            projectId: this.projectId,
          });
        }
      },
    });

    this.scriptsStore.scriptContent.subscribe({
      next: (scriptContent) => {
        this.code = scriptContent;
      },
    });

    this.fileSystemService.getProjectAbsolutePath(this.projectId).subscribe({
      next: (projectAbsolutePath) => {
        this.projectAbsolutePath = projectAbsolutePath;
      },
    });

    this.themeService.darkThemeSubject.subscribe((isDark) => {
      if (isDark) {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-dark' };
      } else {
        this.editorOptions = { ...this.editorOptions, theme: 'vs-light' };
      }
    });

    this.scriptsStore.loadAvailableLanguages();
    this.scriptsStore.availableLanguages.subscribe({
      next: (languages) => {
        this.availableScriptLanguages = languages;
        this.setEditorLanguage(this.scriptPath);
      },
    });
  }

  onRunScriptButtonClick() {
    this.isLoadingResults = true;
    this.runScriptService.runScriptFromPath(this.projectId, this.scriptPath, this.getLanguage(this.scriptPath)).subscribe({
      next: (run) => {
        this.isLoadingResults = false;
        // todo: remove this global store and use a component store

        if (run.results.filter((r) => r.type === 'RunError').length > 0) {
          this.snackBar.open('Script run has errors!', 'Ok', {
            duration: 4000,
          });
        }

        this.store.dispatch(
          setOutput({
            runIndex: run.index,
            scriptName: run.scriptName,
            results: run.results,
          })
        );
      },
      error: (err) => {
        this.isLoadingResults = false;

        if (err.status === 404 && err.error.includes('Could not find project')) {
          this.snackBar.open('Project Context might not be loaded!', 'Ok', {
            duration: 4000,
          });
          return;
        }

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
    const scriptParameters = this.scriptsStore.getScriptParameters(this.projectId, this.scriptPath);

    this.dialog.open(EditParametersDialogComponent, {
      disableClose: true,
      data: {
        scriptId: this.scriptPath,
        projectId: this.projectId,
        parameters: scriptParameters,
      },
    });
  }
}

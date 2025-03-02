import { Component, computed, input, OnInit, signal } from '@angular/core';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../../../../services/theme/theme.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltip } from '@angular/material/tooltip';
import { CdkCopyToClipboard } from '@angular/cdk/clipboard';
import { SafeUrlPipe } from '../../../../../../pipes/safe-url.pipe';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { EditParametersDialogComponent } from './edit-parameters-dialog/edit-parameters-dialog.component';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss'],
  imports: [EditorComponent, MatButtonModule, MatIconModule, MatTooltip, CdkCopyToClipboard, SafeUrlPipe, FormsModule],
})
export class SelectedScriptComponent implements OnInit {
  projectId = input.required<string>();
  filePath = input.required<string>();

  editorOptions = computed(() => {
    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: 'csharp',
      readOnly: true,
      automaticLayout: true,
    };
  });

  code = signal<string>('');

  // TODO FIXIT: absolute path computed
  // TODO FIXIT: parameters computed
  projectAbsolutePath = signal('abc');
  // isLoadingResults = false;
  // scriptContentError$ = this.scriptsStore.scriptContentError;
  // scriptContentLoading$ = this.scriptsStore.scriptContentLoading;
  //
  // private availableScriptLanguages: ScriptLanguage[] = [];
  //
  constructor(
    private themeService: ThemeService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  //
  ngOnInit(): void {
    //   this.projectId = this.projectStore.getProjectId();
    //
    //   this.notificationService.watchedFiles.subscribe({
    //     next: (watchedFile) => {
    //       if (this.scriptPath === watchedFile.path) {
    //         this.code = watchedFile.content;
    //       }
    //     },
    //   });
    //
    //   this.route.paramMap.subscribe({
    //     next: (params) => {
    //       const scriptPath = params.get('scriptPath');
    //       this.scriptPath = scriptPath ?? '';
    //
    //       if (scriptPath && this.projectId) {
    //         this.setEditorLanguage(scriptPath);
    //
    //         this.scriptsStore.loadScriptContent({
    //           scriptId: scriptPath,
    //           projectId: this.projectId,
    //         });
    //       }
    //     },
    //   });
    //
    //   this.scriptsStore.scriptContent.subscribe({
    //     next: (scriptContent) => {
    //       this.code = scriptContent;
    //     },
    //   });
    //
    //   this.fileSystemService.getProjectAbsolutePath(this.projectId).subscribe({
    //     next: (projectAbsolutePath) => {
    //       this.projectAbsolutePath = projectAbsolutePath;
    //     },
    //   });
    //
    //   this.themeService.darkThemeSubject.subscribe((isDark) => {
    //     if (isDark) {
    //       this.editorOptions = { ...this.editorOptions, theme: 'vs-dark' };
    //     } else {
    //       this.editorOptions = { ...this.editorOptions, theme: 'vs-light' };
    //     }
    //   });
    //
    //   this.scriptsStore.loadAvailableLanguages();
    //   this.scriptsStore.availableLanguages.subscribe({
    //     next: (languages) => {
    //       this.availableScriptLanguages = languages;
    //       this.setEditorLanguage(this.scriptPath);
    //     },
    //   });
  }

  //
  onRunScriptButtonClick() {
    //   this.isLoadingResults = true;
    //   this.runScriptService.runScriptFromPath(this.projectId, this.scriptPath, this.getLanguage(this.scriptPath)).subscribe({
    //     next: (run) => {
    //       this.isLoadingResults = false;
    //       // todo: remove this global store and use a component store
    //
    //       if (run.results.filter((r) => r.type === 'RunError').length > 0) {
    //         this.snackBar.open('Script run has errors!', 'Ok', {
    //           duration: 4000,
    //         });
    //       }
    //
    //       this.store.dispatch(
    //         setOutput({
    //           runIndex: run.index,
    //           scriptName: run.scriptName,
    //           results: run.results,
    //         })
    //       );
    //     },
    //     error: (err) => {
    //       this.isLoadingResults = false;
    //
    //       if (err.status === 404 && err.error.includes('Could not find project')) {
    //         this.snackBar.open('Project Context might not be loaded!', 'Ok', {
    //           duration: 4000,
    //         });
    //         return;
    //       }
    //
    //       this.snackBar.open('Could not run script!', 'Ok', {
    //         duration: 4000,
    //       });
    //     },
    //   });
  }

  onCopyToClipboardButtonClick() {
    this.snackBar.open('Path copied successfully!', 'Ok', {
      duration: 2000,
    });
  }

  //
  // private setEditorLanguage(scriptPath: string) {
  //   if (!scriptPath) {
  //     return;
  //   }
  //
  //   for (const language of this.availableScriptLanguages) {
  //     if (scriptPath.endsWith(language.extension)) {
  //       this.editorOptions = { ...this.editorOptions, language: language.name };
  //       return;
  //     }
  //   }
  // }
  //
  // private getLanguage(filename: string) {
  //   if (!filename) {
  //     return '';
  //   }
  //
  //   for (const language of this.availableScriptLanguages) {
  //     if (filename.endsWith(language.extension)) {
  //       return language.name;
  //     }
  //   }
  //
  //   return '';
  // }

  onEditParametersButtonClick() {
    this.dialog.open(EditParametersDialogComponent, {
      disableClose: true,
      data: {
        projectId: this.projectId(),
        // TODO FIXIT: pass script parameters
        parameters: [],
      },
    });
  }
}

import {BrowserModule, DomSanitizer} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';
import {FlexLayoutModule} from '@angular/flex-layout';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {AngularSplitModule} from 'angular-split';

import {MatToolbarModule} from '@angular/material/toolbar';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {MatIconModule, MatIconRegistry} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatTableModule} from '@angular/material/table';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatSortModule} from '@angular/material/sort';
import {MatInputModule} from '@angular/material/input';
import {MatDialogModule} from '@angular/material/dialog';
import {MatTabsModule} from '@angular/material/tabs';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatCardModule} from '@angular/material/card';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatSelectModule} from '@angular/material/select';
import {MatTreeModule} from '@angular/material/tree';
import {MatDividerModule} from '@angular/material/divider';
import {MatListModule} from '@angular/material/list';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ClipboardModule} from '@angular/cdk/clipboard';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatChipsModule} from '@angular/material/chips';
import {MatMenuModule} from '@angular/material/menu';
import {MonacoEditorModule} from '@materia-ui/ngx-monaco-editor';

import {ProjectsComponent} from './projects/projects.component';
import {CreateProjectDialogComponent} from './projects/create-project-dialog/create-project-dialog.component';
import {DeleteProjectDialogComponent} from './projects/delete-project-dialog/delete-project-dialog.component';
import {MatRippleModule} from '@angular/material/core';
import {ProjectDetailsComponent} from './project-details/components/project-details.component';
import {ROUTES} from './app-routes';
import {DragAndDropFilesComponent} from './shared/drag-and-drop-files/drag-and-drop-files.component';
import {FileDropDirective} from './shared/file-drop-directive/file-drop.directive';
import {TreeComponent} from './shared/tree/tree.component';
import {DetailsContentComponent} from './project-details/components/details-content/details-content.component';
import {
    ScriptsContentComponent
} from './project-details/components/run-script/scripts-content/scripts-content.component';
import {
    NoScriptsComponent
} from './project-details/components/run-script/scripts-content/no-scripts/no-scripts.component';
import {
    SelectedScriptComponent
} from './project-details/components/run-script/selected-script/selected-script.component';
import {SlugifyPipe} from './shared/slugify.pipe';
import {
    CreateScriptDialogComponent
} from './project-details/components/run-script/create-script-dialog/create-script-dialog.component';
import {SelectableTreeComponent} from './shared/selectable-tree/selectable-tree.component';
import {SafeUrlPipe} from './shared/safe-url/safe-url.pipe';
import {
    ConsoleOutputComponent
} from './project-details/components/run-script/output/console-output/console-output.component';
import {FileOutputComponent} from './project-details/components/run-script/output/file-output/file-output.component';
import {ErrorDialogComponent} from './shared/error-dialog/error-dialog.component';
import {
    LoadingResultsDialogComponent
} from './project-details/components/run-script/selected-script/loading-results-dialog/loading-results-dialog.component';
import {StoreModule} from '@ngrx/store';
import {EffectsModule} from '@ngrx/effects';
import {outputReducer} from './state/outputs/output.reducer';
import {
    OutputErrorsComponent
} from './project-details/components/run-script/output/output-errors/output-errors.component';
import {OutputEffects} from './state/outputs/output.effects';
import {projectDetailsReducer} from './state/project-details/project-details.reducer';
import {ProjectDetailsEffects} from './state/project-details/project-details.effects';
import {
    UploadModelsComponent
} from './project-details/components/details-content/upload-models/upload-models.component';
import {LoadModelsComponent} from './project-details/components/details-content/load-models/load-models.component';
import {LinkModelsComponent} from './project-details/components/details-content/link-models/link-models.component';
import {
    CurrentlyLoadedModelsComponent
} from './project-details/components/details-content/currently-loaded-models/currently-loaded-models.component';
import {
    ProjectContextComponent
} from './project-details/components/details-content/project-context/project-context.component';
import {scriptTreeReducer} from './state/script-tree/script-tree.reducer';
import {
    ScriptTreeComponent
} from './project-details/components/run-script/scripts-content/script-tree/script-tree.component';
import {
    PluginsMarketplaceDashboardComponent
} from './plugins/components/plugins-marketplace-dashboard/plugins-marketplace-dashboard.component';
import {
    PluginMarketplaceDashboardListComponent
} from './plugins/components/plugin-marketplace-dashboard-list/plugin-marketplace-dashboard-list.component';
import {
    PluginMarketplaceDashboardListItemComponent
} from './plugins/components/plugin-marketplace-dashboard-list-item/plugin-marketplace-dashboard-list-item.component';
import {ApiErrorMessageComponent} from './shared/api-error-message/api-error-message.component';
import {
    ScriptParametersListComponent
} from './project-details/components/run-script/script-parameters-list/script-parameters-list.component';
import {
    ScriptParameterComponent
} from './project-details/components/run-script/script-parameters-list/script-parameter/script-parameter.component';
import {CenteredSpinnerComponent} from './shared/centered-spinner/centered-spinner.component';
import {
    EditParametersDialogComponent
} from './project-details/components/run-script/edit-parameters-dialog/edit-parameters-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ProjectsComponent,
    CreateProjectDialogComponent,
    DeleteProjectDialogComponent,
    ProjectDetailsComponent,
    DragAndDropFilesComponent,
    FileDropDirective,
    TreeComponent,
    DetailsContentComponent,
    ScriptsContentComponent,
    NoScriptsComponent,
    SelectedScriptComponent,
    SlugifyPipe,
    CreateScriptDialogComponent,
    SelectableTreeComponent,
    SafeUrlPipe,
    ConsoleOutputComponent,
    FileOutputComponent,
    ErrorDialogComponent,
    LoadingResultsDialogComponent,
    OutputErrorsComponent,
    UploadModelsComponent,
    LoadModelsComponent,
    LinkModelsComponent,
    CurrentlyLoadedModelsComponent,
    ProjectContextComponent,
    ScriptTreeComponent,
    PluginsMarketplaceDashboardComponent,
    PluginMarketplaceDashboardListComponent,
    PluginMarketplaceDashboardListItemComponent,
    ApiErrorMessageComponent,
    ScriptParametersListComponent,
    ScriptParameterComponent,
    CenteredSpinnerComponent,
    EditParametersDialogComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(ROUTES),
    BrowserAnimationsModule,
    FlexLayoutModule,
    MatToolbarModule,
    MatSlideToggleModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatDialogModule,
    MatRippleModule,
    MatTabsModule,
    MatSnackBarModule,
    MatCardModule,
    MatExpansionModule,
    MatSelectModule,
    MatTreeModule,
    MatDividerModule,
    MatListModule,
    MatSidenavModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatMenuModule,
    MonacoEditorModule,
    ClipboardModule,
    AngularSplitModule,
    StoreModule.forRoot(
      {
        outputState: outputReducer,
        projectDetails: projectDetailsReducer,
        scriptTree: scriptTreeReducer,
      },
      {}
    ),
    EffectsModule.forRoot([OutputEffects, ProjectDetailsEffects]),
  ],
  providers: [SlugifyPipe],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(matIconRegistry: MatIconRegistry, domSanitizer: DomSanitizer) {
    matIconRegistry.addSvgIconSet(domSanitizer.bypassSecurityTrustResourceUrl('./assets/mdi.svg'));
  }
}

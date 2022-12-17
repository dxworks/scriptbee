import { BrowserModule, DomSanitizer } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@angular/flex-layout';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularSplitModule } from 'angular-split';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSelectModule } from '@angular/material/select';
import { MatTreeModule } from '@angular/material/tree';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';

import { MonacoEditorModule } from '@materia-ui/ngx-monaco-editor';

import { ProjectsComponent } from './projects/projects.component';
import { CreateProjectDialogComponent } from './projects/create-project-dialog/create-project-dialog.component';
import { DeleteProjectDialogComponent } from './projects/delete-project-dialog/delete-project-dialog.component';
import { MatRippleModule } from '@angular/material/core';
import { ProjectDetailsComponent } from './project-details/project-details.component';
import { ROUTES } from './app-routes';
import { DragAndDropFilesComponent } from './shared/drag-and-drop-files/drag-and-drop-files.component';
import { FileDropDirective } from './shared/file-drop-directive/file-drop.directive';
import { TreeComponent } from './shared/tree/tree.component';
import { DetailsContentComponent } from './project-details/details-content/details-content.component';
import { ScriptsContentComponent } from './project-details/scripts-content/scripts-content.component';
import { NoScriptsComponent } from './project-details/scripts-content/no-scripts/no-scripts.component';
import { SelectedScriptComponent } from './project-details/scripts-content/selected-script/selected-script.component';
import { SlugifyPipe } from './shared/slugify.pipe';
import {
  CreateScriptDialogComponent
} from './project-details/scripts-content/create-script-dialog/create-script-dialog.component';
import { SelectableTreeComponent } from './shared/selectable-tree/selectable-tree.component';
import { SafeUrlPipe } from './shared/safe-url/safe-url.pipe';
import { ConsoleOutputComponent } from './project-details/output/console-output/console-output.component';
import { FileOutputComponent } from './project-details/output/file-output/file-output.component';
import {
  ErrorDialogComponent
} from './project-details/details-content/error-dialog/error-dialog/error-dialog.component';
import {
  LoadingResultsDialogComponent
} from './project-details/scripts-content/selected-script/loading-results-dialog/loading-results-dialog.component';
import { PluginItemComponent } from './plugins/plugin-list/plugin-item/plugin-item.component';
import { PluginListComponent } from './plugins/plugin-list/plugin-list.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { outputReducer } from "./state/outputs/output.reducer";
import { OutputErrorsComponent } from './project-details/output/output-errors/output-errors.component';
import { OutputEffects } from "./state/outputs/output.effects";
import { projectDetailsReducer } from "./state/project-details/project-details.reducer";
import { ProjectDetailsEffects } from "./state/project-details/project-details.effects";
import { UploadModelsComponent } from './project-details/details-content/upload-models/upload-models.component';
import { LoadModelsComponent } from './project-details/details-content/load-models/load-models.component';
import { LinkModelsComponent } from './project-details/details-content/link-models/link-models.component';
import {
  CurrentlyLoadedModelsComponent
} from './project-details/details-content/currently-loaded-models/currently-loaded-models.component';
import { ProjectContextComponent } from './project-details/details-content/project-context/project-context.component';
import { scriptTreeReducer } from "./state/script-tree/script-tree.reducer";
import { ScriptTreeEffects } from "./state/script-tree/script-tree.effects";
import { ScriptTreeComponent } from './project-details/scripts-content/script-tree/script-tree.component';
import { PluginsComponent } from './plugins/plugins/plugins.component';
import { PluginsMarketplaceComponent } from './plugins/plugins-marketplace/plugins-marketplace.component';
import {
  ExpandedPluginRowComponent
} from './plugins/plugins-marketplace/expanded-plugin-row/expanded-plugin-row.component';

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
    PluginItemComponent,
    PluginListComponent,
    OutputErrorsComponent,
    UploadModelsComponent,
    LoadModelsComponent,
    LinkModelsComponent,
    CurrentlyLoadedModelsComponent,
    ProjectContextComponent,
    ScriptTreeComponent,
    PluginsComponent,
    PluginsMarketplaceComponent,
    ExpandedPluginRowComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(ROUTES),
    BrowserAnimationsModule,
    FormsModule,
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
    MonacoEditorModule,
    ClipboardModule,
    AngularSplitModule,
    StoreModule.forRoot({
      outputState: outputReducer,
      projectDetails: projectDetailsReducer,
      scriptTree: scriptTreeReducer,
    }, {}),
    EffectsModule.forRoot([OutputEffects, ProjectDetailsEffects, ScriptTreeEffects]),
  ],
  providers: [
    SlugifyPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(matIconRegistry: MatIconRegistry, domSanitizer: DomSanitizer) {
    matIconRegistry.addSvgIconSet(
      domSanitizer.bypassSecurityTrustResourceUrl('./assets/mdi.svg')
    );
  }
}

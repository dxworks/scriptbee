import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';
import {FlexLayoutModule} from '@angular/flex-layout';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import {MatToolbarModule} from '@angular/material/toolbar';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {MatIconModule} from '@angular/material/icon';
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

import {MonacoEditorModule} from '@materia-ui/ngx-monaco-editor';

import {ProjectsComponent} from './projects/projects.component';
import {CreateProjectDialogComponent} from './projects/create-project-dialog/create-project-dialog.component';
import {DeleteProjectDialogComponent} from './projects/delete-project-dialog/delete-project-dialog.component';
import {MatRippleModule} from '@angular/material/core';
import {ProjectDetailsComponent} from './project-details/project-details.component';
import {ROUTES} from './app-routes';
import {DragAndDropFilesComponent} from './shared/drag-and-drop-files/drag-and-drop-files.component';
import {FileDropDirective} from './shared/file-drop-directive/file-drop.directive';
import {TreeComponent} from './shared/tree/tree.component';
import {DetailsContentComponent} from './project-details/details-content/details-content.component';
import {ScriptsContentComponent} from './project-details/scripts-content/scripts-content.component';
import {NoScriptsComponent} from './project-details/scripts-content/no-scripts/no-scripts.component';
import {SelectedScriptComponent} from './project-details/scripts-content/selected-script/selected-script.component';
import {SlugifyPipe} from './shared/slugify.pipe';
import { CreateScriptDialogComponent } from './project-details/scripts-content/create-script-dialog/create-script-dialog.component';
import { SelectableTreeComponent } from './shared/selectable-tree/selectable-tree.component';
import { SafeUrlPipe } from './shared/safe-url/safe-url.pipe';

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
    SafeUrlPipe
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(ROUTES),
    BrowserAnimationsModule,
    MatToolbarModule,
    MatSlideToggleModule,
    MatIconModule,
    MatButtonModule,
    FormsModule,
    FlexLayoutModule,
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
    MonacoEditorModule,
    MatTooltipModule,
    ClipboardModule,
    MatCheckboxModule
  ],
  providers: [
    SlugifyPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}

import { Component, input, signal } from '@angular/core';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { LoaderService } from '../../../../../services/loaders/loader.service';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { DragAndDropFilesComponent } from '../../../../../components/drag-and-drop-files/drag-and-drop-files.component';
import { MatButtonModule } from '@angular/material/button';
import { apiHandler } from '../../../../../utils/apiHandler';
import { UploadService } from '../../../../../services/upload/upload.service';

@Component({
  selector: 'app-upload-models',
  templateUrl: './upload-models.component.html',
  styleUrls: ['./upload-models.component.scss'],
  imports: [
    CenteredSpinnerComponent,
    ErrorStateComponent,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    DragAndDropFilesComponent,
    MatButtonModule,
  ],
})
export class UploadModelsComponent {
  projectId = input.required<string>();

  selectedLoaderId = signal<string | undefined>(undefined);

  getLoadersResource = createRxResourceHandler({
    loader: () => this.loaderService.getAllLoaders(),
  });

  uploadModelsHandler = apiHandler(
    (params: { loaderId: string; projectId: string; files: File[] }) => this.uploadService.uploadModels(params.loaderId, params.projectId, params.files),
    (data) => {
      console.log(data);
    }
  );

  files: File[] = [];

  constructor(
    private loaderService: LoaderService,
    private uploadService: UploadService
  ) {}

  onUploadFilesClick() {
    const loaderId = this.selectedLoaderId();
    if (loaderId) {
      this.uploadModelsHandler.execute({
        projectId: this.projectId(),
        loaderId: loaderId,
        files: this.files,
      });
    }
  }
}

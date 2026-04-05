import { Component, computed, inject, input, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { LoaderService } from '../../../../../services/loaders/loader.service';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { DragAndDropFilesComponent } from '../../../../../components/drag-and-drop-files/drag-and-drop-files.component';
import { MatButtonModule } from '@angular/material/button';
import { UploadService } from '../../../../../services/upload/upload.service';
import { finalize } from 'rxjs';
import { convertError } from '../../../../../utils/api';

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
  instanceId = input.required<string>();

  selectedLoaderId = signal<string | undefined>(undefined);

  getLoadersResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      instanceId: this.instanceId(),
    }),
    stream: ({ params }) => this.loaderService.getAllLoaders(params.projectId, params.instanceId),
  });
  getLoadersResourceError = computed(() => convertError(this.getLoadersResource.error()));

  isUploadLoading = signal(false);

  files: File[] = [];

  private loaderService = inject(LoaderService);
  private uploadService = inject(UploadService);

  onUploadFilesClick() {
    const loaderId = this.selectedLoaderId();
    if (loaderId) {
      this.isUploadLoading.set(true);
      this.uploadService
        .uploadModels(this.projectId(), loaderId, this.files)
        .pipe(finalize(() => this.isUploadLoading.set(false)))
        .subscribe({
          next: (data) => {
            console.log(data);
          },
        });
    }
  }
}

import { Component, computed, inject, signal } from '@angular/core';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { convertError } from '../../../../../utils/api';
import { LoaderService } from '../../../../../services/loaders/loader.service';
import { UploadService } from '../../../../../services/upload/upload.service';
import { finalize } from 'rxjs';
import { DragAndDropFilesComponent } from '../../../../../components/drag-and-drop-files/drag-and-drop-files.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-upload-model-page',
  imports: [
    ErrorStateComponent,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    DragAndDropFilesComponent,
    MatButtonModule,
    LoadingProgressBarComponent,
  ],
  templateUrl: './upload-model-page.component.html',
  styleUrl: './upload-model-page.component.scss',
})
export class UploadModelPage {
  private projectStateService = inject(ProjectStateService);

  projectId = computed(() => this.projectStateService.currentProjectId()!);
  instanceId = computed(() => this.projectStateService.currentInstanceId());

  selectedLoaderId = signal<string | undefined>(undefined);

  getLoadersResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      instanceId: this.instanceId()!,
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

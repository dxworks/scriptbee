import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { FileDropDirective } from '../../directives/file-drop-directive/file-drop.directive';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-drag-and-drop-files',
  templateUrl: './drag-and-drop-files.component.html',
  styleUrls: ['./drag-and-drop-files.component.scss'],
  imports: [MatIcon, FileDropDirective, MatButtonModule, MatListModule],
})
export class DragAndDropFilesComponent {
  @ViewChild('modelFileInput') modelFileInput?: ElementRef;
  @Output() filesChange = new EventEmitter<File[]>();

  isHovering: boolean = false;
  @Input() files: File[] = [];

  toggleHover(event: boolean) {
    this.isHovering = event;
  }

  onFileChange(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement && inputElement.files) {
      this.addFiles(inputElement.files);
    }
  }

  addFiles(files: FileList) {
    for (let i = 0; i < files.length; i++) {
      const file = files.item(i);
      if (file) {
        this.files.push(file);
      }
    }
    this.filesChange.emit(this.files);
  }

  openBrowseFilesDialog() {
    if (!this.modelFileInput) {
      return;
    }

    this.modelFileInput.nativeElement.value = '';
    this.modelFileInput.nativeElement.click();
  }

  onClearButtonClick(index: number) {
    this.files.splice(index, 1);
  }
}

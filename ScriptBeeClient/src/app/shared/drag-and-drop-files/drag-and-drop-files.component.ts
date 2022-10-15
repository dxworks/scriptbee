import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'app-drag-and-drop-files',
  templateUrl: './drag-and-drop-files.component.html',
  styleUrls: ['./drag-and-drop-files.component.scss']
})
export class DragAndDropFilesComponent {

  @ViewChild('modelFileInput') modelFileInput?: ElementRef;
  @Output() filesChange = new EventEmitter<File[]>();

  isHovering: boolean = false;
  @Input() files: File[] = [];

  toggleHover(event: boolean) {
    this.isHovering = event;
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

    this.modelFileInput.nativeElement.value = "";
    this.modelFileInput.nativeElement.click();
  }

  onClearButtonClick(index) {
    this.files.splice(index, 1);
  }
}

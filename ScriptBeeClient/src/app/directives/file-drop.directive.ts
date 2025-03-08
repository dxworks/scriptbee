import { Directive, EventEmitter, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[appFileDrop]',
})
export class FileDropDirective {
  @Output() filesDropped = new EventEmitter<FileList>();
  @Output() filesHovered = new EventEmitter<boolean>();

  @HostListener('drop', ['$event'])
  onDrop($event: any) {
    $event.preventDefault();

    const transfer = $event.dataTransfer;
    this.filesDropped.emit(transfer.files);
    this.filesHovered.emit(false);
  }

  @HostListener('dragover', ['$event'])
  onDragOver($event: Event) {
    $event.preventDefault();
    this.filesHovered.emit(true);
  }

  @HostListener('dragleave', ['$event'])
  onDragLeave($event: Event) {
    $event.preventDefault();
    this.filesHovered.emit(false);
  }
}

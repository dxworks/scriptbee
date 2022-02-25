import {Injectable} from '@angular/core';
import {LoaderService} from "../loader/loader.service";
import {TreeNode} from "../../shared/tree-node";

@Injectable({
  providedIn: 'root'
})
export class SaveFilesService {

  savedFilesDictionary = new Map<string, File[]>();

  constructor(private loaderService: LoaderService) {
    this.loaderService.getAllLoaders().subscribe(loaders => {
      if (loaders) {
        loaders.forEach(loader => {
          this.savedFilesDictionary.set(loader, []);
        });
      }
    });
  }

  updateSavedFilesDictionary(loader: string, files: File[]) {
    const loaderFiles = this.savedFilesDictionary.get(loader);
    if (loaderFiles) {
      files.forEach(file => {
        loaderFiles.push(file);
      });
    }
  }

  getSavedFiles(): TreeNode[] {
    return Array.from(this.savedFilesDictionary.entries(), ([name, files]) => {
      return {
        name: name,
        children: files
      }
    });
  }
}

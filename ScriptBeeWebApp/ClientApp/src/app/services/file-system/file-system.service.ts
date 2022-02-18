import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {FileTreeNode} from '../../project-details/scripts-content/fileTreeNode';

@Injectable({
  providedIn: 'root'
})
export class FileSystemService {

  constructor() { }
  
  getFileSystem() : Observable<FileTreeNode[]> {
    
  }
  
  getFileContent(projectId: string, filePath: string) : Observable<string>{
    
  }
}

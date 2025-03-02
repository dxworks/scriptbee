export interface ReturnedProject {
  id: string;
  name: string;
  creationDate: string;
  savedFiles: ReturnedNode[];
  loadedFiles: ReturnedNode[];
  loaders: string[];
  linker: string;
}

export interface ReturnedNode {
  loaderName: string;
  files: string[];
}

export class UserFolderPathService {
  public static getUserFolderPath(projectId: string) {
    const path = localStorage.getItem(getUserFolderPathKey(projectId));
    if (path === null) {
      return localStorage.getItem(previousCreateProjectKey()) ?? 'C:\\Users\\user\\.scriptbee';
    }
    return path;
  }

  public static setUserFolderPath(projectId: string, value: string) {
    localStorage.setItem(getUserFolderPathKey(projectId), value);
  }

  public static removeUserFolderPath(projectId: string) {
    localStorage.removeItem(getUserFolderPathKey(projectId));
  }

  public static setPreviousCreateProjectPath(value: string) {
    localStorage.setItem(previousCreateProjectKey(), value);
  }

  public static getPreviousCreateProjectPath() {
    return localStorage.getItem(previousCreateProjectKey());
  }
}

function getUserFolderPathKey(projectId: string) {
  return `user-folder-path-${projectId}`;
}

function previousCreateProjectKey() {
  return `previous-user-folder-path`;
}

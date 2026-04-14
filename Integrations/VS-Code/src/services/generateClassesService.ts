import * as path from 'path';
import * as fs from 'fs/promises';
import axiosInstance from '../api/axiosInstance';
import { getProjectGeneratedPath } from '../utils/workspaceUtils';
import { logger } from '../utils/logger';

export class GenerateClassesService {
  public async generate(baseUrl: string, projectId: string, instanceId: string, languages: string[] = []): Promise<void> {
    const generatedPath = getProjectGeneratedPath(projectId);

    await this.clearGeneratedPath(projectId);

    const response = await axiosInstance.post(
      `/api/projects/${projectId}/instances/${instanceId}/context/generate-classes`,
      { languages },
      {
        baseURL: baseUrl,
        responseType: 'arraybuffer',
      }
    );

    logger.log(`Received ${response.data.byteLength} bytes from backend for class generation.`);
    logger.log(`Target generated path: ${generatedPath}`);

    await this.writeClassesToFiles(Buffer.from(response.data), generatedPath);
  }

  private async writeClassesToFiles(data: Buffer, generatedPath: string): Promise<void> {
    let offset = 0;

    while (offset < data.length) {
      if (offset + 4 > data.length) {
        break;
      }
      const pathLength = data.readInt32BE(offset);
      offset += 4;

      if (pathLength === 0) {
        break;
      }

      if (offset + pathLength > data.length) {
        break;
      }
      const relativePath = data.toString('utf8', offset, offset + pathLength);
      offset += pathLength;

      if (offset + 8 > data.length) {
        break;
      }
      const contentLength = Number(data.readBigInt64BE(offset));
      offset += 8;

      if (offset + contentLength > data.length) {
        break;
      }
      const content = data.subarray(offset, offset + contentLength);
      offset += contentLength;

      const fullPath = path.join(generatedPath, relativePath);
      logger.log(`Writing generated class: ${fullPath} (${content.length} bytes)`);
      await fs.mkdir(path.dirname(fullPath), { recursive: true });
      await fs.writeFile(fullPath, content);
    }
  }

  private async clearGeneratedPath(projectId: string): Promise<void> {
    const generatedPath = getProjectGeneratedPath(projectId);

    try {
      await fs.rm(generatedPath, { recursive: true, force: true });
    } catch {}
    await fs.mkdir(generatedPath, { recursive: true });
  }
}

export const generateClassesService = new GenerateClassesService();

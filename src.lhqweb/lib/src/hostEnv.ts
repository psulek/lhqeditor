import * as path from 'path';
import { textEncode } from './utils';

export interface IHostEnvironment {
    debugLog(msg: string): void;
    // addResultFile(name: string, content: string, bom: boolean, lineEndings: LineEndings): void;
    // addModelGroupSettings(group: string, settings: unknown): void;
    pathCombine(path1: string, path2: string): string;
    webHtmlEncode(input: string): string;
    stopwatchStart(): number;
    stopwatchEnd(start: number): string;
}

declare global {
    var HostEnvironment: IHostEnvironment;
}

export class HostEnvironmentDefault implements IHostEnvironment {
    //private _generatedFiles: GeneratedFile[] = [];

    public debugLog(msg: string): void {
        console.log(msg);
    }

    // public addResultFile(name: string, content: string, bom: boolean, lineEndings: LineEndings) {
    //     this._generatedFiles.push(new GeneratedFile(name, content, bom, lineEndings));
    // }

    // public addModelGroupSettings(group: string, settings: unknown) {
    //     const json = JSON.stringify(settings);
    //     console.log(`Added model group '${group}' with settings: ` + json);
    // }

    public pathCombine(path1: string, path2: string): string {
        return path.join(path1, path2);
    }

    public webHtmlEncode(input: string): string {
        return textEncode(input, { mode: 'html' });
    }

    public stopwatchStart(): number {
        return performance.now();
    }

    public stopwatchEnd(start: number): string {
        return `${(performance.now() - start).toFixed(2)}ms`;
    }
}
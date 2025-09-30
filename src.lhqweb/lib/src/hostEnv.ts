import type { IHostEnvironment } from './types';

import { textEncode } from './utils';

export class HostEnvironment implements IHostEnvironment {
    public debugLog(msg: string): void {
        console.log(msg);
    }

    public pathCombine(path1: string, path2: string): string {
        return `${path1 ?? ''}/` + (path2 ?? '');
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
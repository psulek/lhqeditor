import * as path from 'path';
import * as utils from '../src/utils.mjs';

import type { IHostEnvironment } from '../src/types.mjs';

export class HostEnvironmentDefault implements IHostEnvironment {
    public debugLog(msg: string): void {
        console.log(msg);
    }

    public pathCombine(path1: string, path2: string): string {
        return path.join(path1, path2);
    }

    public webHtmlEncode(input: string): string {
        return utils.textEncode(input, { mode: 'html' });
    }

    public stopwatchStart(): number {
        return performance.now();
    }

    public stopwatchEnd(start: number): string {
        return `${(performance.now() - start).toFixed(2)}ms`;
    }
}
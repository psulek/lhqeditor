import { formatDuration } from './utils';

export class Duration {
    private _start: number;
    private _end: number | undefined;
    
    constructor() {
        this._start = Date.now();
    }

    public static start(): Duration {
        return new Duration();
    }

    public end(): void {
        this._end = Date.now();
    }

    public get elapsed(): number {
        const now = this._end ?? Date.now();
        return now - this._start;
    }

    public get elapsedTime(): string {
        return formatDuration(this.elapsed);
    }
}
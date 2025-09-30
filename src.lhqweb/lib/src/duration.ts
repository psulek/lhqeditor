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
        return Duration.formatDuration(this.elapsed);
    }

    public static formatDuration(ms: number): string {
        const seconds = Math.floor(ms / 1000);
        const milliseconds = ms % 1000;
    
        return seconds > 0
            ? `${seconds} second${seconds > 1 ? 's' : ''} and ${milliseconds} ms`
            : `${milliseconds} ms`;
    }
}
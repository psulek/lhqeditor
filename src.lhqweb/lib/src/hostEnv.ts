// declare let HostEnv: {
//     DebugLog: (msg: string) => void;
//     AddResultFile: (name: string, content: string) => void;
// }

import {type LineEndings} from "./types";

declare function HostDebugLog(msg: string): void;
declare function HostAddResultFile(name: string, content: string, bom: boolean, lineEndings: LineEndings): void;
declare function HostAddModelGroupSettings(group:string, settings: string): void;
declare function HostPathCombine(path1: string, path2: string): string;
declare function HostWebHtmlEncode(input: string): string;
declare function HostStopwatchStart(): number;
declare function HostStopwatchEnd(start: number): string;

export class HostEnv {
    public static addResultFile(name: string, content: string, bom: boolean, lineEndings: LineEndings) {
        if (HostAddResultFile) {
            HostAddResultFile(name, content, bom, lineEndings);
            //HostEnv.debugLog(`Added file '${name}' with content: ${content}`);
        } else {
            console.log(`Added file '${name}' with content: '${content}'`);
        }
    }
    
    public static addModelGroupSettings(group: string, settings: unknown) {
        const json = JSON.stringify(settings);
        if (HostAddModelGroupSettings) {
            HostAddModelGroupSettings(group, json);
        } else {
            console.log(`Added model group '${group}' with settings: ` + json);
        }
    }
    
    public static debugLog(msg: string) {
        if (HostDebugLog) {
            HostDebugLog(msg);
        } else {
            console.log(msg);
        }
    }
    
    public static pathCombine(path1: string, path2: string): string {
        if (HostPathCombine) {
            return HostPathCombine(path1, path2);
        }
        
        return path1 + '/' + path2;
    }
    
    public static webHtmlEncode(input: string): string {
        if (HostWebHtmlEncode) {
            return HostWebHtmlEncode(input);
        }
        
        return input;
    }
    
    public static stopwatchStart(): number {
        if (HostStopwatchStart) {
            return HostStopwatchStart();
        }
        
        return Date.now();
    }
    
    public static stopwatchEnd(start: number): string {
        if (HostStopwatchEnd) {
            return HostStopwatchEnd(start);
        }
        
        return (Date.now() - start).toString();
    }
}
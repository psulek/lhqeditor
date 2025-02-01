// declare let HostEnv: {
//     DebugLog: (msg: string) => void;
//     AddResultFile: (name: string, content: string) => void;
// }

declare function HostDebugLog(msg: string): void;
declare function HostAddResultFile(name: string, content: string): void;
declare function HostPathCombine(path1: string, path2: string): string;
declare function HostWebHtmlEncode(input: string): string;

export class HostEnv {
    public static addResultFile(name: string, content: string) {
        if (HostAddResultFile) {
            HostAddResultFile(name, content);
            //HostEnv.debugLog(`Added file '${name}' with content: ${content}`);
        } else {
            console.log(`Added file '${name}' with content: '${content}'`);
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
}
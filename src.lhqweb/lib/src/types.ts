export type HbsTemplatesData = {
    [templateId: string]: string;
};

export interface IHostEnvironment {
    debugLog(msg: string): void;
    pathCombine(path1: string, path2: string): string;
    webHtmlEncode(input: string): string;
    stopwatchStart(): number;
    stopwatchEnd(start: number): string;
}

export type GeneratorInitialization = {
    /**
     * Handlebars templates, where each key represents 'templateId' (unique identifier) and value represents handlebars template content.
     */
    hbsTemplates: HbsTemplatesData;

    /**
     * Host environment with which generator interacts when running code templates.
     */
    hostEnvironment: IHostEnvironment;
}
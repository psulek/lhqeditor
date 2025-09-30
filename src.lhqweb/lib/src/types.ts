import type { LhqModel } from './api/schemas';

export type HbsTemplatesData = {
    [templateId: string]: string;
};

export type KeysMatching<T, V> = { [K in keyof T]-?: T[K] extends V ? K : never }[keyof T];

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

/**
 * Represents the result of a model validation.
 */
export type LhqValidationResult = {
    success: boolean, error: string | undefined, model?: LhqModel;
}

export type CSharpNamespaceInfo = {
    csProjectFileName: string;
    t4FileName: string;
    namespace: string | undefined;
    referencedLhqFile: boolean;
    referencedT4File: boolean;
};

export type TextEncodeOptions =
    { mode: 'html' } |
    { mode: 'xml', quotes: boolean } |
    { mode: 'json' };

export type TextEncodeModes = Extract<TextEncodeOptions, { mode: unknown }>['mode'];
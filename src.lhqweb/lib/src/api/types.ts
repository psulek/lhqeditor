import type { LhqModel, LhqModelLineEndings } from './schemas';

// export type GeneratorSource = {
//     model: LhqModel;
//     fileName: string;
//     csProjectFileName: string;
//     // outDir: string;
// };



/**
 * Represents information about a single generated file as a result of the Generator.Generate method. 
 */
export type GeneratedFile = {
    /**
     * File name of generated file, usually relative path to source *.lhq model file.
     */
    fileName: string;

    /**
     * Generated content string that should be written to fileName.
     */
    content: string;

    /**
     * BOM marker that should be used when writing content to fileName file.
     */
    bom: boolean;

    /**
     * Line endings that should be used when writing content to fileName file.
     */
    lineEndings: LhqModelLineEndings;
};

/**
 * Represents result of code generator process of Generator.generate.
 */
export type GenerateResult = {
    /**
     * List of generated files.
     */
    generatedFiles: Readonly<GeneratedFile[]>;
}
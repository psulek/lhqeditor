import { LhqModelLineEndings } from './model/api';

/**
 * Represents information about a single generated file as a result of the Generator.Generate method.
 */
export class GeneratedFile {
    private _fileName: string;
    private _content: string;
    private _bom: boolean;
    private _lineEndings: LhqModelLineEndings;

    private static regexLF = new RegExp("\\r\\n|\\r", "g");
    private static regexCRLF = new RegExp("(\\r(?!\\n))|((?<!\\r)\\n)", "g");

    /**
     * Initializes a new instance of the GeneratedFile class.
     * @param fileName File name of generated file, usually relative path to source *.lhq model file.
     * @param content Generated content string that should be written to fileName.
     * @param bom BOM marker that should be used when writing content to fileName file.
     * @param lineEndings Line endings that should be used when writing content to fileName file.
     */
    constructor(fileName: string, content: string, bom: boolean, lineEndings: LhqModelLineEndings) {
        this._fileName = fileName;
        this._content = content;
        this._bom = bom;
        this._lineEndings = lineEndings;
    }

    /**
     * Gets the file name of the generated file, usually a relative path to the source *.lhq model file.
     */
    public get fileName(): string {
        return this._fileName;
    }

    /**
     * Gets a flag indicating whether to write a BOM marker when saving content to the fileName file.
     */
    public get bom(): boolean {
        return this._bom;
    }

    /**
     * Gets the generated content string that should be written to the fileName file.
     * 
     * @param applyLineEndings Flag indicating whether to apply line endings to the content or not.
     * @returns The generated content string, without any modifications if applyLineEndings is false, or with line endings applied if applyLineEndings is true.
     */
    public getContent(applyLineEndings: boolean): string {
        if (!applyLineEndings || this._content.length === 0) {
            return this._content;
        }

        return this._lineEndings === 'lf'
            ? this._content.replace(GeneratedFile.regexLF, "\n")
            : this._content.replace(GeneratedFile.regexCRLF, "\r\n");
    }
}
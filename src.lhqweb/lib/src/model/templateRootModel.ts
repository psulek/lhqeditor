import { AppError } from '../AppError';
import { isNullOrEmpty } from '../utils';
import { LhqModelCodeGeneratorBasicSettings } from './api/schemas';
import { IRootModelElement } from './api/types';

export type OutputFileData = {
    fileName: string;
    settings: LhqModelCodeGeneratorBasicSettings;
};

// model which is bind to handlebars template compile and run
export class TemplateRootModel {
    private _model: IRootModelElement;
    private _data: Record<string, unknown>;
    private _host: Record<string, unknown>;
    private _rootOutputFile: OutputFileData | undefined;

    constructor(model: IRootModelElement, data: Record<string, unknown>, host: Record<string, unknown>) {
        if (isNullOrEmpty(model)) {
            throw new AppError('Missing root model !');
        }

        this._model = model;
        this._data = data ?? {};
        this._host = host ?? {};
    }

    public setRootOutputFile(outputFile: OutputFileData): void {
        if (!isNullOrEmpty(this._rootOutputFile)) {
            throw new AppError('Root output file already set !');
        }

        this._rootOutputFile = outputFile;
    }

    public get rootOutputFile(): OutputFileData | undefined {
        return this._rootOutputFile;
    }

    public addToTempData = (key: string, value: unknown): void => {
        this._data[key] = value;
    }

    public clearTempData = (): void => {
        this._data = {};
    }

    /**
     * loaded lhq model file as parsed json structure
     */
    public get model(): IRootModelElement {
        return this._model;
    }

    /**
     * extra data defined dynamically by template run, resets on each template run.
     */
    public get data(): Readonly<Record<string, unknown>> {
        return this._data;
    }

    /*
     * data from host environment, stays for all template runs within the same session.
     */
    public get host(): Record<string, unknown> {
        return this._host;
    }
};
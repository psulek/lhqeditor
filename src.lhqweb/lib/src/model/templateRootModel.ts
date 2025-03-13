import { AppError } from '../AppError';
import { isNullOrEmpty } from '../utils';
import { CodeGeneratorBasicSettings, ICategoryLikeTreeElement, IRootModelElement, ITreeElement } from './api/types';
import { TreeElement } from './treeElement';

export type OutputFileData = {
    fileName: string | undefined;
    settings: CodeGeneratorBasicSettings | undefined;
};

export type OutputInlineData = OutputFileData & { content: string };

export type OutputChildData = {
    templateId: string;
    host: Record<string, unknown> | undefined;
}

export type TemplateRunType = 'root' | 'child';

// model which is bind to handlebars template compile and run
export class TemplateRootModel {
    private _model: IRootModelElement;
    private _data: Record<string, unknown>;
    private _rootHost: Record<string, unknown> | undefined;
    private _host: Record<string, unknown>;
    private _output: OutputFileData | undefined;
    private _templateRunType: TemplateRunType = 'root';
    private _childOutputs: OutputChildData[] = [];
    private _inlineOutputs: OutputInlineData[] = [];

    constructor(model: IRootModelElement, data: Record<string, unknown>, host: Record<string, unknown>) {
        if (isNullOrEmpty(model)) {
            throw new AppError('Missing root model !');
        }

        this._model = model;
        this._data = data ?? {};
        this._host = host ?? {};
    }

    public setOutput(outputFile: OutputFileData): void {
        // if (!isNullOrEmpty(this._output)) {
        //     throw new AppError('Output was already set !');
        // }

        // if (this._templateRunType !== 'root') {
        //     throw new AppError('Child template cannot set main output !');
        // }

        this._output = outputFile;
    }

    public addChildOutput(templateId: string, host: Record<string, unknown> | undefined) {
        if (this._templateRunType === 'child') {
            throw new AppError('Child template could not have other child outputs !');
        }

        this._childOutputs.push({ templateId, host });
    }

    public addInlineOutputs(inlineOutput: OutputInlineData) {
        this._inlineOutputs.push(inlineOutput);
    }

    public get childOutputs(): OutputChildData[] {
        return this._childOutputs;
    }

    public get inlineOutputs(): OutputInlineData[] {
        return this._inlineOutputs;
    }

    public get templateRunType(): TemplateRunType {
        return this._templateRunType;
    }

    public setAsChildTemplate(childData: OutputChildData): void {
        if (this._templateRunType === 'root') {
            this._rootHost = Object.freeze(Object.assign({}, this._host ?? {}));
            this._templateRunType = 'child';
        }

        this.clearTempData();
        //this._childOutputs = [];
        this._inlineOutputs = [];
        this._host = Object.assign({}, childData.host ?? {}, this._rootHost);
        this._output = undefined;

        const recursiveClear = (element: ICategoryLikeTreeElement) => {
            if (element instanceof TreeElement) {
                element.clearTempData();
            }

            if (element.hasCategories) {
                element.categories.forEach(recursiveClear);
            }

            if (element.hasResources) {
                element.resources.forEach(e => {
                    if (e instanceof TreeElement) {
                        e.clearTempData();
                    }
                });
            }
        };

        recursiveClear(this.model);
    }

    public get output(): OutputFileData | undefined {
        return this._output;
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

    public get settings(): Readonly<CodeGeneratorBasicSettings> {
        const settings = this._output?.settings;
        if (isNullOrEmpty(settings)) {
            throw new AppError('Missing root output file settings !');
        }

        return settings;
    }

    /*
     * data from host environment, stays for all template runs within the same session.
     */
    public get host(): Record<string, unknown> {
        return this._host;
    }
};
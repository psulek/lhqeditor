import { ITreeElement, ITreeElementPaths } from './types';

export class TreeElementPaths implements ITreeElementPaths {
    private paths: string[] = [];

    constructor(element: ITreeElement) {
        if (element.parent) {
            const parentPaths = element.parent.paths;
            
            if (parentPaths instanceof TreeElementPaths) {
                this.paths = [...parentPaths.paths];
            }
        }

        this.paths.push(element.name);
    }

    public getParentPath(separator: string): string {
        separator ??= '';
        return this.paths.join(separator);
    }
}

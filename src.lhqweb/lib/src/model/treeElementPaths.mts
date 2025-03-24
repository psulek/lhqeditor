import type { ITreeElement, ITreeElementPaths } from '../api/modelTypes.mjs';

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

    public getParentPath = (separator: string, includeRoot?: boolean): string => {
        separator ??= '';
        includeRoot ??= false;
        // return this.paths.join(separator);
        return (includeRoot || this.paths.length === 1)
            ? this.paths.join(separator)
            : this.paths.slice(1).join(separator);
    }
}

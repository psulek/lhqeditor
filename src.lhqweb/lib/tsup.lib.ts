import { defineConfig } from 'tsup';

export default defineConfig((options) => {
    return {
        entry: ['src/index.ts'], // Entry point
        outDir: '', // Output directory
        format: 'iife', // Immediately Invoked Function Expression (IIFE) for browser compatibility
        globalName: 'LhqGenerators', // Equivalent to `library.name` in Webpack
        target: 'es2015', // Target ES2015
        // minify: !options.watch,
        minify: false,
        platform: 'browser', // Target browser environment
        sourcemap: false, // Equivalent to `devtool: false` in Webpack
        dts: false, // Disable .d.ts file generation
        clean: false, // Clean the output directory before building
        tsconfig: 'tsconfig.json', // Use the specified tsconfig.json
        esbuildOptions(options, ctx) {
            // Ensure ES2015 module and lib settings
            options.target =  'es2015';
            options.outfile = 'dist/index.js';
            options.supported = {
                'arrow': true,
                'const-and-let': true,
                'for-of': true,
                'array-spread': true,
                'destructuring': true,
                'default-argument': true,
                'template-literal': true,
                //'modules': true,
            };
        },
    };
});
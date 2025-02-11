const path = require('path');

module.exports = {
  entry: './src/index.ts',
  mode: 'production',
  output: {
    library: {
        name: 'LhqGenerators',
        type: 'var'
    },
    filename: 'lhqgenerators.js',
    path: path.resolve(__dirname, 'dist')
  },
  target: 'web',
  devtool: false,
  resolve: {
    extensions: ['.ts', '.js']
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  }
};
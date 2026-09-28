const path = require('node:path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const webpack = require('webpack');

/**
 * Webpack build configuration for the Scholarship CMGroups React client.
 *
 * This branch uses Webpack + Yarn (not Vite/npm). Production bundles are emitted to `dist/`.
 * `DefinePlugin` inlines the public API base URL at build time — never put secrets here.
 */
module.exports = (_env, argv) => {
  const isProduction = argv.mode === 'production';

  return {
    entry: './src/index.tsx',
    output: {
      path: path.resolve(__dirname, 'dist'),
      filename: isProduction ? 'assets/[name].[contenthash:8].js' : 'assets/[name].js',
      chunkFilename: isProduction ? 'assets/[name].[contenthash:8].js' : 'assets/[name].js',
      publicPath: '/',
      clean: true,
    },
    resolve: {
      extensions: ['.tsx', '.ts', '.js'],
    },
    module: {
      rules: [
        {
          test: /\.tsx?$/,
          use: {
            loader: 'ts-loader',
            options: {
              configFile: 'tsconfig.build.json',
            },
          },
          exclude: /node_modules/,
        },
        {
          test: /\.css$/i,
          use: ['style-loader', 'css-loader'],
        },
      ],
    },
    plugins: [
      new HtmlWebpackPlugin({
        template: path.resolve(__dirname, 'src/index.html'),
      }),
      new webpack.DefinePlugin({
        'process.env.API_BASE_URL': JSON.stringify(process.env.API_BASE_URL ?? ''),
      }),
    ],
    devServer: {
      port: 5173,
      historyApiFallback: true,
      static: {
        directory: path.join(__dirname, 'dist'),
      },
      proxy: [
        {
          context: ['/api'],
          target: 'https://localhost:7148',
          changeOrigin: true,
          secure: false,
        },
      ],
    },
    devtool: isProduction ? 'hidden-source-map' : 'eval-source-map',
    optimization: {
      splitChunks: {
        chunks: 'all',
      },
    },
    performance: {
      maxEntrypointSize: 600000,
      maxAssetSize: 600000,
    },
  };
};

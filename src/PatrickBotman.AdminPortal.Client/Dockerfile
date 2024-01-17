# Stage 1: Compile and Build angular codebase

# Use official node image as the base image
FROM node:latest as build

# Set the working directory
WORKDIR /usr/local/app

# Add the source code to app
COPY ./patrickbotman-admin-portal/ /usr/local/app/

# Install all the dependencies
RUN npm install 

# Generate the build of the application
RUN npm run build --   --configuration=production --base-href /admin/


# Stage 2: Serve app with nginx server

# Use official nginx image as the base image
FROM nginx:latest

# Copy the build output to replace the default nginx contents.
COPY ./patrickbotman-admin-portal/nginx/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /usr/local/app/dist/patrickbotman-admin-portal/browser /usr/share/nginx/html

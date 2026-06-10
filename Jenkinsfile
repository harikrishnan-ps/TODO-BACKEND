 pipeline {
    agent any
 
    environment {
        CONTAINER_NAME = 'todo-backend'
        IMAGE_NAME = 'todo-app-backend'
        NETWORK_NAME = 'todo-network'
        DB_CONTAINER = 'todo-mysql-db'
        PORT_MAPPING = '5000:8080'
        DB_CONNECTION = "Server=todo-mysql-db;Port=3306;Database=todo_db;User=root;Password=root_password;"
        JWT_SECRET = 'SuperSecretKeyForTodoAppAuthJWTToken2026'
        JWT_ISSUER = 'TodoApi'
        JWT_AUDIENCE = 'TodoUi'
    }
 
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
 
        stage('Build Docker Image') {
            steps {
                bat "docker build --no-cache -t ${IMAGE_NAME}:latest -t ${IMAGE_NAME}:${BUILD_NUMBER} ."
            }
        }
 
        stage('Deploy Container') {
            steps {
                script {
                    // Ensure Docker network exists (ignoring error if it already exists)
                    bat "docker network create ${NETWORK_NAME} 2>nul || ver >nul"
                   
                    // Stop and remove existing container if running
                    bat "docker stop ${CONTAINER_NAME} 2>nul || ver >nul"
                    bat "docker rm ${CONTAINER_NAME} 2>nul || ver >nul"
                   
                    // Launch new container using Windows Batch line continuation
                    bat """
                        docker run -d ^
                            --name ${CONTAINER_NAME} ^
                            --network ${NETWORK_NAME} ^
                            -p ${PORT_MAPPING} ^
                            -e DB_CONNECTION_STRING="${DB_CONNECTION}" ^
                            -e JWT_SECRET="${JWT_SECRET}" ^
                            -e JWT_ISSUER="${JWT_ISSUER}" ^
                            -e JWT_AUDIENCE="${JWT_AUDIENCE}" ^
                            -e RUN_MIGRATIONS=true ^
                            -e ENABLE_SWAGGER=true ^
                            ${IMAGE_NAME}:latest
                    """
                }
            }
        }
    }
 
    post {
        success {
            echo "Backend pipeline completed successfully!"
        }
        failure {
            echo "Backend pipeline failed. Please check the logs."
        }
    }
}
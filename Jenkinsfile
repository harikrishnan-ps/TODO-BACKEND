pipeline {
    agent any

    environment {
        // Defines the configuration to build (Debug or Release)
        BUILD_CONFIGURATION = 'Release'
    }

    stages {
        stage('Restore') {
            steps {
                echo 'Restoring NuGet packages...'
                sh 'dotnet restore TodoApp.Api/TodoApp.Api.csproj'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the application...'
                sh 'dotnet build TodoApp.Api/TodoApp.Api.csproj --configuration ${BUILD_CONFIGURATION} --no-restore'
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                // If you add a test project later, uncomment this line:
                // sh 'dotnet test --configuration ${BUILD_CONFIGURATION} --no-build'
                echo 'No test project configured yet. Skipping.'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo 'Building Docker image...'
                sh 'docker build -t todo-backend:latest -t todo-backend:${BUILD_NUMBER} .'
            }
        }
    }

    post {
        always {
            cleanWs()
        }
        success {
            echo "✅ Backend Pipeline completed successfully!"
        }
        failure {
            echo "❌ Backend Pipeline failed. Please check the logs."
        }
    }
}

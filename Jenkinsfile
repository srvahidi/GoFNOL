@Library('scm-shared-library@common-lib') _
// PodTemplates varibales
def label = "jenkins-pod"
def cloudId = 'kubernetes'
def namespace = 'jenkins'
def branchName = "${env.BRANCH_NAME}"

//Git
def repoName = "gofnol"

//docker build
def imageName = "rms/automated_appraisal_engine/gofnol"
def dockerRegistry = "feeds.axadmin.net/docker"
def credentialsIdDocker = "SA-WW-ProgetArtifa"
def Dockerfile = "gofnol.dockerfile"
def DockerfilePath = "ci/docker"

//Kubectl deploy
def deploynamespace = "rms-dev-a2e"
def pathToYML = "./ci/tanzu/acceptance"

//VAULT
def VAULT_INSTANCE = "dev.usdc01"
def VAULT_SECRETS_PATH = 'concourse/rms/dev-cluster'
def VAULT_SECRETS_PATH_Teams = 'secret/hari_test'

//Jenkins workspace
workingdir = "/home/jenkins"

def images = [jnlp:"jenkins/inbound-agent:4.7-1", custom1:"feeds.axadmin.net/docker/rms/automated_appraisal_engine/gofnol-sdk-image:latest", cust1podCpuLmt:"1", cust1podMemLmt:"2Gi", custom2:"feeds.axadmin.net/docker/rms/automated_appraisal_engine/gofnol-node-image:latest",cust2podCpuLmt:"1", cust2podMemLmt:"2Gi", custom3:"feeds.axadmin.net/docker/debian",custom4:"feeds.axadmin.net/docker/rms/automated_appraisal_engine/gofnol-utility-image",docker:"docker:20-dind", helm:"feeds.axadmin.net/docker/scm/helm:3.9.0-scm"]
properties([disableConcurrentBuilds(),buildDiscarder(logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '', numToKeepStr: '20'))])
try {
    nodeTemplate = new PodTemplates(cloudId, namespace, label, images, workingdir, this)
    echo "Container images: ${images}"
    echo "running agents on node with label ${label}"
    nodeTemplate.BuilderTemplate {
        node(nodeTemplate.podlabel) {
            timestamps {
               stage ("checkout"){
                container('jnlp') {
                  milestone()
                  try{
				  	if(env.CHANGE_ID ) {
				  	     checkout scm
				       }else{
                         git branch: "${env.BRANCH_NAME}", credentialsId: "HSSoleraNABB", url: "https://bitbucket.org/SoleraNA/${repoName}.git"
					 }
                  }                
                  catch(e) {
                       currentBuild.result = "ABORTED"
                       def latestcommitHash = sh(script: "git rev-parse origin/${branchName}", returnStdout: true).trim()
                       println "Latest commit hash: ${latestcommitHash}"
                       error('!!!! Git commit hash code differs cannot continue with build !!!!')                    
						}
					}
				}
				if(branchName =~ /(master|jenkins_migration)/) {									
				stage("build-GoFNOL"){
				container('custom1') {
				sh '''
				
				          output_directory=${PWD}/GoFNOL-binaries
						  # cd ./GoFNOL-git
						  CI=false dotnet publish ./GoFNOL/GoFNOL.csproj -c Release -o ${output_directory}/app
						  # format the version so that we can use lexicographic search to find the latest
						  version=$(git describe --always --tags | awk -F- '{printf "%s-%04d-%s", $1, $2, $3}')
						  echo $version > ${output_directory}/app/version.txt
						  cp ./ci/manifest.yml ${output_directory}/manifest.yml
						  tar -xvf "./ci/docker/binfiles/newrelic-netcore20-agent_8.21.34.0_amd64.tar.gz" -C ${output_directory}/app
				
				'''
					
					}
					}

				}
				if(branchName =~ /(master|jenkins_migration)/) {								
				stage("test-GoFNOL.frontendTest"){
				container('custom2') {
				configFileProvider([configFile(fileId: 'feeds_axadmin_npm', variable: 'NPM_SETTINGS')]) {
				def npmSettings = readFile("${env.NPM_SETTINGS}")
				writeFile file: '.npmrc',text:npmSettings
				sh '''
                    mv .npmrc ./GoFNOL/ClientApp/.npmrc
					cd ./GoFNOL/ClientApp
					npm ci
					CI=true npm test
				
				'''
					}
					}
					}

				}
				if(branchName =~ /(master|jenkins_migration)/) {
				stage("test-GoFNOL.backendtests"){
				container('custom1') {
				sh '''
					  export VCAP_SERVICES=$(cat ./ci/cicd_env.json)
					  export ASPNETCORE_ENVIRONMENT=Staging
					  dotnet test ./GoFNOL.tests/GoFNOL.tests.csproj
				
				'''
					}
					}

				}
				if(branchName =~ /(master|jenkins_migration)/) {
				stage("prepare-k8s"){
				container('custom3') {
				sh '''
					  set -x
					  DOCKER_TAG=`cat GoFNOL-binaries/app/version.txt`
					  sed -i \"s/:latest/:$DOCKER_TAG/\" ci/tanzu/acceptance/gofnol-blue-deployment.yaml
					  cat ci/tanzu/acceptance/gofnol-blue-deployment.yaml | grep \"image:\"
					  
				  '''
					
					}
					}

				}
				if(env.CHANGE_ID ) {
                stage ("sonar-scan"){
                container('custom1') {
                    //sonarScan(credentialsIdSonar)
					sh "echo 'SONAR SCANNING'"
					}
                    }
					}
				if(branchName =~ /(master|jenkins_migration)/) {	
				stage("publish-GoFNOL"){
				container('custom4') {
				sh '''
					  cd ${PWD}/GoFNOL-binaries
					  version=$(cat ./app/version.txt)
					  filename=GoFNOL-${version}.zip
					  zip -r ${filename} ./*
					 mkdir ../artifact
					 mv ${filename} ../artifact/${filename}
				
				'''
					
					}
					}

				}
				if(branchName =~ /(master|jenkins_migration)/) {				
				stage("docker-build"){
				container('docker') {
				def imageVersion = readFile('GoFNOL-binaries/app/version.txt').trim()
				dockerrms(dockerRegistry,credentialsIdDocker,imageName,imageVersion,Dockerfile,DockerfilePath)
					
					}
					}
				}
				if(branchName =~ /(master|jenkins_migration)/) {				
				stage("kubectl-deploy-acceptance"){
                container('helm'){				
                    kubectlDeploy(VAULT_INSTANCE,VAULT_SECRETS_PATH,deploynamespace,pathToYML)
					
					}
					}
				}				
			}
        }
    }
}catch(e) {
    currentBuild.result = "FAILED"
    echo 'BUILD FAILED'
    throw e
}finally {
    TeamsNotify(VAULT_INSTANCE,VAULT_SECRETS_PATH_Teams)
    buildNotification{
        emailId = """harinath.sangala@solera.com"""
    }
}
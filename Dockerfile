# This is now a multi-stage build file which fetches the required jar files directly

# Stage 1 downloads the otp jar file
FROM ubuntu:bionic as build

WORKDIR /opt/build
RUN apt-get update && \
  apt-get install --yes curl && \
  apt-get clean
RUN curl -Lo otp.jar https://repo1.maven.org/maven2/org/opentripplanner/otp/1.4.0/otp-1.4.0-shaded.jar

# Stage 2 creates the runtime
FROM openjdk:10

WORKDIR /opt/otp4gb
COPY --from=build /opt/build/otp.jar .
COPY graphs ./graphs

EXPOSE 8080

CMD [ "java", "-Xmx4G", \
  "-jar", "otp.jar", \
  "--router", "centredonsouthyorkshire", \
  "--graphs", "graphs", \
  "--server" ]